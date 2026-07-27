package com.tawala.integration.ysl;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URL;
import java.net.URLConnection;
import java.net.URLEncoder;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Date;
import java.util.Iterator;
import java.util.List;
import java.util.UUID;

import org.dom4j.Document;
import org.dom4j.DocumentException;
import org.dom4j.DocumentHelper;
import org.dom4j.Element;
import org.dom4j.io.SAXReader;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;

import com.scissor.Log;
import com.scissor.StreamCopier;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.FormSubmission;
import com.tawala.project.UserProject;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.RecordSelector.FormDataProvider;
import com.tawala.web.WorldInitializer;

public class LeagueDataExchange {
	private static final Reference SCHEDULE_URL_REFERENCE = new Reference(
			"AdminSetup", "ScheduleURL");
	// -- See ysl-config.xml for details.
	private static String SENDER_PASSWORD;
	private static String SENDER_ID;
	private static String POST_URL;

	public void setPostURL(String postURL) {
		LeagueDataExchange.POST_URL = postURL;
	}

	public void setSenderId(String senderId) {
		LeagueDataExchange.SENDER_ID = senderId;
	}

	public void setSenderPassword(String senderPassword) {
		LeagueDataExchange.SENDER_PASSWORD = senderPassword;
	}

	private static final DateFormat DATE_FORMAT = new SimpleDateFormat(
			"MM/dd/yyyy");

	@SuppressWarnings("unchecked")
	public static List<String> updateYSL(final UserProject userProject) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (List<String>) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						List<String> messages = new ArrayList<String>();

						League league = extractDataFrom(userProject);
						if (league == null) {
							return Collections
									.singletonList("The league is not properly set up. "
											+ "Please make sure that the Admin Setup is complete before proceeding.");
						}

						String payload = LeagueDataExchange.generatePayload(
								userProject, league);

						ByteArrayOutputStream outputStream = new ByteArrayOutputStream();
						try {
							URL url = new URL(POST_URL);

							URLConnection connection = url.openConnection();
							connection.setDoInput(true);
							connection.setDoOutput(true);

							String message = "message="
									+ URLEncoder.encode(payload, "UTF-8");

							OutputStream out = connection.getOutputStream();
							try {
								StreamCopier.copy(new ByteArrayInputStream(
										message.getBytes("UTF-8")), out);
							} finally {
								out.close();
							}

							Document document = getResponseData(connection,
									outputStream);

							processesResponse(document, league, messages);

							userProject.setYSLLastUpdated(new Date());
							TawalaSessionFactory.MAIN.getHibernateTemplate()
									.update(userProject);
						} catch (Exception e) {
							Log
									.error(
											this,
											"Error synchronizing with YourSportsLeague.com:",
											e);
							if (outputStream.size() > 0) {
								Log.error(this,
										"Failed payload: "
												+ new String(outputStream
														.toByteArray()));
							}
							status.setRollbackOnly();
							messages
									.add("There was an error updating YourSportsLeague.com data. Please try again.");
						}

						return messages;
					}

					private Document getResponseData(URLConnection connection,
							ByteArrayOutputStream outputStream)
							throws IOException, DocumentException {
						Document document;
						InputStream inputStream = connection.getInputStream();
						StreamCopier.copy(inputStream, outputStream);

						try {
							SAXReader saxReader = new SAXReader();
							saxReader.setMergeAdjacentText(true);
							document = saxReader.read(new ByteArrayInputStream(
									outputStream.toByteArray()));
						} finally {
							inputStream.close();
						}
						return document;
					}

				});
	}

	@SuppressWarnings("unchecked")
	public static void processesResponse(Document document, League league,
			List<String> messages) {
		Element root = document.getRootElement();
		Iterator<Element> responseIterator = root.elementIterator("Response");
		while (responseIterator.hasNext()) {
			Element response = responseIterator.next();
			String successCode = response.attributeValue("code");
			if (successCode == null) {
				Log.error(LeagueDataExchange.class,
						"'code' attribute is not found");
			}
			if (successCode.equals("0")) {
				String type = response.attributeValue("elementType");
				if (type == null) {
					Log.error(LeagueDataExchange.class,
							"'elementType' attribute is not found");
				} else if (type.equals("League")) {
					processLeagueResponse(league, response);
				} else if (type.equals("Team")) {
					processTeamResponse(league, response);
				}
			} else {
				String message = response.attributeValue("errorMessage");
				if (message == null) {
					Log.error(LeagueDataExchange.class,
							"'errorMessage' attribute is not found");
				} else {
					messages.add(message);
				}
			}
		}

	}

	private static void processLeagueResponse(League league, Element response) {
		String leagueURL = response.attributeValue("leagueURL");
		if (leagueURL == null) {
			Log.error(LeagueDataExchange.class,
					"'leagueURL' attribute is not found");
			return;
		}

		if (!leagueURL.equals(league.getSetupRecord().getValue(
				SCHEDULE_URL_REFERENCE).toString())) {
			league.getSetupRecord().setValue(
					SCHEDULE_URL_REFERENCE.getFieldName(), leagueURL);
			WorldInitializer.getDefaultWorld().domain().storedData().update(
					league.getSetupRecord());
		}
	}

	private static void processTeamResponse(League league, Element response) {
		String teamId = response.attributeValue("elementId");
		if (teamId == null) {
			Log.error(LeagueDataExchange.class,
					"'elementId' attribute is not found");
			return;
		}

		Team team = league.getTeamByUniqueId(teamId);
		if (team == null) {
			Log.error(LeagueDataExchange.class, "Team with id " + teamId
					+ " is not found.");
			return;
		}

		String teamURL = response.attributeValue("teamURL");
		if (teamURL == null) {
			Log.error(LeagueDataExchange.class, "No URL for team with id "
					+ teamId + ".");
			return;
		}

		FormSubmission teamRecord = team.getRecord();
		Reference teamURLReference = new Reference("Team", "ScheduleURL");

		if (!teamURL.equals(teamRecord.getValue(teamURLReference).toString())) {
			teamRecord.setValue(teamURLReference.getFieldName(), teamURL);
			WorldInitializer.getDefaultWorld().domain().storedData().update(
					teamRecord);
		}
	}

	public static String generatePayload(UserProject userProject, League league) {
		Document document = DocumentHelper.createDocument();
		Element root = document.addElement("YSLIntegrationData");
		root.addAttribute("id", "0001");
		root.addAttribute("sender", SENDER_ID);
		root.addAttribute("password", SENDER_PASSWORD);
		root.addAttribute("date", DATE_FORMAT.format(new Date()));

		Element leagueElement = root.addElement("League");
		leagueElement.addAttribute("id", String.valueOf(userProject.getId()));
		leagueElement.addAttribute("name", league.getName());
		leagueElement.addAttribute("yslId", userProject.getYSLLeagueId());

		for (Team team : league.getTeams()) {
			Element teamElement = leagueElement.addElement("Team");
			teamElement.addAttribute("id", team.getUniqueId());
			teamElement.addAttribute("name", team.getTeamName());
			teamElement.addAttribute("gender", "Unknown");
			teamElement.addAttribute("ageGroup", "Unknown");
			teamElement.addAttribute("playClass", "Unknown");

			for (Coach coach : team.getCoaches()) {
				Element coachElement = teamElement.addElement("Coach");
				coachElement.addAttribute("id", coach.getUniqueId());
				addNonEmptyTextElement(coachElement, "firstName", coach
						.getFirstName());
				addNonEmptyTextElement(coachElement, "lastName", coach
						.getLastName());
				addNonEmptyTextElement(coachElement, "middleInitial", coach
						.getMiddleName());
				addNonEmptyTextElement(coachElement, "streetAddress", coach
						.getAddress());
				addNonEmptyTextElement(coachElement, "city", coach.getCity());
				addNonEmptyTextElement(coachElement, "state", coach.getState());
				addNonEmptyTextElement(coachElement, "postalCode", coach
						.getPostalCode());
				addNonEmptyTextElement(coachElement, "country", coach
						.getCountry());
				addNonEmptyTextElement(coachElement, "email", coach.getEmail());
				addNonEmptyTextElement(coachElement, "phone", coach.getPhone());
			}
		}

		return document.asXML();
	}

	public static League extractDataFrom(UserProject userProject) {
		ExecutionContext context = new ExecutionContext(WorldInitializer
				.getDefaultWorld().domain(), userProject);

		League league = retrieveLeagueData(context, userProject);
		if (league == null) {
			return null;
		}

		retrieveTeams(context, league);
		retrieveCoaches(context, league);

		return league;
	}

	private static League retrieveLeagueData(ExecutionContext context,
			UserProject userProject) {
		RecordSelector setupRecordSelector = new RecordSelector(
				Collections
						.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
								SCHEDULE_URL_REFERENCE.getFormName())),
				BooleanExpression.TRUE, RecordSelector.DEFAULT_RECORD_LIST_NAME);

		List<CompositeFormSubmission> setupRecords = setupRecordSelector
				.getRecords(context);
		if (setupRecords == null || setupRecords.size() == 0) {
			return null;
		}

		for (CompositeFormSubmission compositeFormSubmission : setupRecords) {
			FormSubmission setupRecord = compositeFormSubmission
					.getFormSubmission(SCHEDULE_URL_REFERENCE);

			return new League(userProject.getName(), setupRecord);
		}

		return null;
	}

	private static void retrieveCoaches(ExecutionContext context, League league) {
		// --- Retrieve coaches
		RecordSelector coachSelector = new RecordSelector(
				Collections
						.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
								"Coach")), BooleanExpression.TRUE,
				RecordSelector.DEFAULT_RECORD_LIST_NAME);

		Reference teamIdReference = new Reference("Coach", "Team");

		List<CompositeFormSubmission> coachRecords = coachSelector
				.getRecords(context);
		if (coachRecords == null || coachRecords.size() == 0) {
			return;
		}

		for (CompositeFormSubmission compositeFormSubmission : coachRecords) {
			FormSubmission coachRecord = compositeFormSubmission
					.getFormSubmission(teamIdReference);

			String teamId = coachRecord.getValue(teamIdReference).toString();
			Team team = league.getTeamById(teamId);
			if (team == null) {
				continue;
			}
			Coach coach = new Coach(coachRecord.getDatabaseId());
			coach.setAddress(coachRecord.getValue(
					new Reference("Coach", "Street")).toString());
			coach.setCity(coachRecord.getValue(new Reference("Coach", "City"))
					.toString());
			coach.setState(coachRecord
					.getValue(new Reference("Coach", "State")).toString());
			coach.setCountry("");
			coach.setPostalCode(coachRecord.getValue(
					new Reference("Coach", "ZIP")).toString());
			coach.setEmail(coachRecord
					.getValue(new Reference("Coach", "Email")).toString());
			coach.setFirstName(coachRecord.getValue(
					new Reference("Coach", "FirstName")).toString());
			coach.setLastName(coachRecord.getValue(
					new Reference("Coach", "LastName")).toString());
			coach.setMiddleName("");
			coach.setPhone(coachRecord.getValue(
					new Reference("Coach", "Phone1")).toString());
			String coachUniqueId = coachRecord.getValue(
					new Reference("Coach", "UniqueID")).toString();
			if (coachUniqueId.length() == 0) {
				coachUniqueId = UUID.randomUUID().toString();
				coachRecord.setValue("UniqueID", coachUniqueId);
				WorldInitializer.getDefaultWorld().domain().storedData()
						.update(coachRecord);
			}
			coach.setUniqueId(coachUniqueId);

			team.addCoach(coach);
		}
	}

	private static void retrieveTeams(ExecutionContext context, League league) {
		RecordSelector teamSelector = new RecordSelector(
				Collections
						.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
								"Team")), BooleanExpression.TRUE,
				RecordSelector.DEFAULT_RECORD_LIST_NAME);
		List<CompositeFormSubmission> teamRecords = teamSelector
				.getRecords(context);
		if (teamRecords == null || teamRecords.size() == 0) {
			return;
		}

		Reference teamNameReference = new Reference("Team", "TeamId");
		Reference teamIdReference = new Reference("Team", "UniqueID");

		for (CompositeFormSubmission compositeFormSubmission : teamRecords) {
			FormSubmission teamRecord = compositeFormSubmission
					.getFormSubmission(teamNameReference);
			Team team = new Team(teamRecord);
			team.setTeamName(teamRecord.getValue(teamNameReference).toString());
			String teamUniqueId = teamRecord.getValue(teamIdReference)
					.toString();
			if (teamUniqueId.length() == 0) {
				teamUniqueId = UUID.randomUUID().toString();
				teamRecord.setValue("UniqueID", teamUniqueId);
				WorldInitializer.getDefaultWorld().domain().storedData()
						.update(teamRecord);
			}

			team.setUniqueId(teamUniqueId);

			league.addTeam(team);
		}
	}

	private static void addNonEmptyTextElement(Element parent,
			String elementName, String value) {
		if (value == null || value.trim().length() == 0) {
			return;
		}
		Element newElement = parent.addElement(elementName);
		newElement.setText(value);
	}
}
