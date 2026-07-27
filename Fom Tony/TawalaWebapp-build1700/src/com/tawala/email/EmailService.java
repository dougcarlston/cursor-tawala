package com.tawala.email;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.sql.SQLException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import javax.mail.internet.MimeMessage;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFCellStyle;
import org.apache.poi.hssf.usermodel.HSSFDataFormat;
import org.apache.poi.hssf.usermodel.HSSFFont;
import org.apache.poi.hssf.usermodel.HSSFRichTextString;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.apache.poi.hssf.util.HSSFColor;
import org.apache.poi.hssf.util.Region;
import org.hibernate.HibernateException;
import org.hibernate.Query;
import org.hibernate.Session;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.orm.hibernate3.HibernateCallback;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.project.UserProject;

public class EmailService {
	private static boolean sendImmediately = false;

	public static void saveEmail(final Email email) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				TawalaSessionFactory.MAIN.getHibernateTemplate().saveOrUpdate(
						email);
				return null;
			}
		});
	}

	@SuppressWarnings("unchecked")
	public static UserProjectEmail getUserProjectEmailById(final User user,
			final long emailId) {

		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (UserProjectEmail) transactionTemplate
				.execute(new TransactionCallback() {

					public Object doInTransaction(TransactionStatus status) {
						List result = TawalaSessionFactory.MAIN
								.getHibernateTemplate()
								.find(
										"from "
												+ UserProjectEmail.class
														.getName()
												+ " where userProject.user = ? and id = ?"
												+ " order by createdDate",
										new Object[] { user, emailId });
						return result.size() == 0 ? null : result.get(0);
					}
				});
	}

	public static int deleteAllEmailsForProject(final long userProjectId) {
		return (Integer)TawalaSessionFactory.MAIN.getHibernateTemplate().execute(
				new HibernateCallback() {

					public Object doInHibernate(Session session)
							throws HibernateException, SQLException {
						Query query = session.createQuery("delete "
								+ UserProjectEmail.class.getName()
								+ " where userProject.id = ?");
						query.setParameter(0, userProjectId);
						return query.executeUpdate();
					}
				});
	}

	public static List<UserProjectEmail> getAllEmailsForProject(
			final User user, final long userProjectId) {
		return getNextPageOfEmailsForProject(user, userProjectId, -1,
				Integer.MAX_VALUE);
	}

	@SuppressWarnings("unchecked")
	public static List<UserProjectEmail> getNextPageOfEmailsForProject(
			final User user, final long userProjectId, final int startIndex,
			final int maxRowCount) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (List<UserProjectEmail>) transactionTemplate
				.execute(new TransactionCallback() {

					public Object doInTransaction(final TransactionStatus status) {
						return TawalaSessionFactory.MAIN.getHibernateTemplate()
								.executeFind(new HibernateCallback() {

									public Object doInHibernate(Session session)
											throws HibernateException,
											SQLException {
										Query query = session
												.createQuery("select new UserProjectEmail(email.id, email.from, email.to, email.cc, email.subject, email.state, email.customerErrorReason, email.errorReason, email.createdDate, email.sentDate) "
														+ " from "
														+ UserProjectEmail.class
																.getName()
														+ " email where email.userProject.user = ? and email.userProject.id = ?"
														+ " order by email.createdDate desc");
										query.setParameter(0, user);
										query.setParameter(1, userProjectId);
										if (startIndex > -1) {
											query.setFirstResult(startIndex);
											query.setMaxResults(maxRowCount);
										}

										return query.list();
									}
								});

					}
				});
	}

	public static long getProjectEmailCount(User user, long userProjectId) {
		return (Long) TawalaSessionFactory.MAIN
				.getHibernateTemplate()
				.find(
						"select count(*) "
								+ " from "
								+ UserProjectEmail.class.getName()
								+ " email where email.userProject.user = ? and email.userProject.id = ?",
						new Object[] { user, userProjectId }).get(0);
	}

	public static long getProjectEmailCount(long userProjectId) {
		return (Long) TawalaSessionFactory.MAIN.getHibernateTemplate().find(
				"select count(*) " + " from "
						+ UserProjectEmail.class.getName()
						+ " email where email.userProject.id = ?",
				new Object[] { userProjectId }).get(0);
	}

	public static void sendAndStoreEmail(Email email) {
		try {
			JavaMailSender sender = Emailer.getSender();
			try {
				MimeMessage message = sender.createMimeMessage();
				sender.send(email.toMimeMessage(message));
				email.markAsSent();
				try {
					saveEmail(email);
				} catch (Throwable e) {
					Log.error(EmailService.class,
							"Error persisting email to the database: ", e);
				}
			} catch (Exception e) {
				Log.error(EmailService.class,
						"Unexpected error sending message: " + e.getMessage(),
						e);
				StringWriter writer = new StringWriter();
				e.printStackTrace(new PrintWriter(writer));

				email.setState(Email.State.ERROR);
				email.setErrorReason(writer.toString().substring(0, 1000));
				email
						.setCustomerErrorReason("Server error occurred during the delivery.");
				try {
					saveEmail(email);
				} catch (Throwable lastOne) {
					Log
							.error(EmailService.class,
									"Error persisting email to the database: ",
									lastOne);
				}
			}
		} catch (Throwable e) {
			Log.warn(EmailService.class, "Failed to send email", e);
		}
	}

	public static HSSFWorkbook createProjectEmailReport(UserProject userProject) {

		// -- Create the workbook and styles.
		final HSSFWorkbook workbook = new HSSFWorkbook();
		final HSSFSheet sheet = workbook.createSheet("Emails");
		sheet.setDisplayGridlines(false);

		HSSFCellStyle defaultDataCellStyle = workbook.createCellStyle();
		defaultDataCellStyle.setBorderBottom(HSSFCellStyle.BORDER_DOTTED);
		defaultDataCellStyle
				.setBottomBorderColor(HSSFColor.GREY_25_PERCENT.index);
		defaultDataCellStyle.setBorderLeft(HSSFCellStyle.BORDER_DOTTED);
		defaultDataCellStyle
				.setLeftBorderColor(HSSFColor.GREY_25_PERCENT.index);

		HSSFCellStyle dateTimeCellStyle = workbook.createCellStyle();
		dateTimeCellStyle.setBorderBottom(HSSFCellStyle.BORDER_DOTTED);
		dateTimeCellStyle.setBottomBorderColor(HSSFColor.GREY_25_PERCENT.index);
		dateTimeCellStyle.setBorderLeft(HSSFCellStyle.BORDER_DOTTED);
		dateTimeCellStyle.setLeftBorderColor(HSSFColor.GREY_25_PERCENT.index);
		dateTimeCellStyle.setDataFormat(HSSFDataFormat
				.getBuiltinFormat("m/d/yy h:mm"));

		int rowCount = -1;

		final short totalColumnCount = 7;

		// --- Add Title
		HSSFCellStyle titleCellStyle = workbook.createCellStyle();
		titleCellStyle.setWrapText(true);
		titleCellStyle.setAlignment(HSSFCellStyle.ALIGN_CENTER);
		titleCellStyle.setVerticalAlignment(HSSFCellStyle.VERTICAL_CENTER);

		HSSFFont titleFont = workbook.createFont();
		titleFont.setBoldweight(HSSFFont.BOLDWEIGHT_BOLD);
		titleFont.setFontHeightInPoints((short) 12);
		titleFont.setItalic(true);
		titleCellStyle.setFont(titleFont);

		HSSFRow row = sheet.createRow(++rowCount);
		HSSFCell cell = row.createCell((short) 0);
		cell
				.setCellValue(new HSSFRichTextString("Project Emails for "
						+ userProject.getUser().getId() + ", "
						+ userProject.getName()));
		cell.setCellStyle(titleCellStyle);
		row.setHeightInPoints(30);

		sheet.addMergedRegion(new Region(rowCount, (short) 0, rowCount,
				totalColumnCount));

		HSSFCellStyle reportDescriptionCellStyle = workbook.createCellStyle();
		reportDescriptionCellStyle.setWrapText(true);
		reportDescriptionCellStyle.setAlignment(HSSFCellStyle.ALIGN_RIGHT);
		reportDescriptionCellStyle
				.setVerticalAlignment(HSSFCellStyle.VERTICAL_CENTER);

		HSSFFont reportDescriptionFont = workbook.createFont();
		reportDescriptionFont.setFontHeightInPoints((short) 10);
		reportDescriptionFont.setItalic(true);
		reportDescriptionFont.setColor(HSSFColor.GREY_50_PERCENT.index);
		reportDescriptionCellStyle.setFont(reportDescriptionFont);

		row = sheet.createRow(++rowCount);
		cell = row.createCell((short) 0);
		cell.setCellStyle(reportDescriptionCellStyle);
		cell.setCellValue(new HSSFRichTextString("Generated on "
				+ new SimpleDateFormat("EEEE MMM d, yyyy").format(new Date())));
		row.setHeightInPoints(20);
		sheet.addMergedRegion(new Region(rowCount, (short) 0, rowCount,
				totalColumnCount));

		// --- Add header
		HSSFCellStyle headerCellStyle = workbook.createCellStyle();
		headerCellStyle.setBorderBottom(HSSFCellStyle.BORDER_DOUBLE);
		headerCellStyle.setBorderTop(HSSFCellStyle.BORDER_DOUBLE);
		headerCellStyle.setBorderLeft(HSSFCellStyle.BORDER_THIN);
		headerCellStyle.setAlignment(HSSFCellStyle.ALIGN_CENTER);
		headerCellStyle.setWrapText(true);

		HSSFFont headerFont = workbook.createFont();
		headerFont.setBoldweight(HSSFFont.BOLDWEIGHT_BOLD);
		headerFont.setFontHeightInPoints((short) 10);
		headerCellStyle.setFont(headerFont);

		short cellNumber = 0;
		HSSFRow headerRow = sheet.createRow(++rowCount);
		cell = headerRow.createCell(cellNumber);
		cell.setCellStyle(headerCellStyle);
		cell.setCellValue(new HSSFRichTextString("From"));
		sheet.setColumnWidth(cellNumber, (short) (30 * 256));

		cellNumber++;
		cell = headerRow.createCell(cellNumber);
		cell.setCellStyle(headerCellStyle);
		cell.setCellValue(new HSSFRichTextString("To"));
		sheet.setColumnWidth(cellNumber, (short) (20 * 256));

		cellNumber++;
		cell = headerRow.createCell(cellNumber);
		cell.setCellStyle(headerCellStyle);
		cell.setCellValue(new HSSFRichTextString("Cc"));
		sheet.setColumnWidth(cellNumber, (short) (20 * 256));

		cellNumber++;
		cell = headerRow.createCell(cellNumber);
		cell.setCellStyle(headerCellStyle);
		cell.setCellValue(new HSSFRichTextString("Subject"));
		sheet.setColumnWidth(cellNumber, (short) (30 * 256));

		cellNumber++;
		cell = headerRow.createCell(cellNumber);
		cell.setCellStyle(headerCellStyle);
		cell.setCellValue(new HSSFRichTextString("State"));
		sheet.setColumnWidth(cellNumber, (short) (10 * 256));

		cellNumber++;
		cell = headerRow.createCell(cellNumber);
		cell.setCellStyle(headerCellStyle);
		cell.setCellValue(new HSSFRichTextString("Error Reason"));
		sheet.setColumnWidth(cellNumber, (short) (20 * 256));

		cellNumber++;
		cell = headerRow.createCell(cellNumber);
		cell.setCellStyle(headerCellStyle);
		cell.setCellValue(new HSSFRichTextString("Created Date"));
		sheet.setColumnWidth(cellNumber, (short) (18 * 256));

		cellNumber++;
		cell = headerRow.createCell(cellNumber);
		cell.setCellStyle(headerCellStyle);
		cell.setCellValue(new HSSFRichTextString("Sent Date"));
		sheet.setColumnWidth(cellNumber, (short) (18 * 256));

		List<UserProjectEmail> emails = getAllEmailsForProject(userProject
				.getUser(), userProject.getId());
		for (UserProjectEmail email : emails) {
			row = sheet.createRow(++rowCount);

			// --- From
			cellNumber = 0;
			cell = row.createCell(cellNumber);
			cell.setCellValue(new HSSFRichTextString(email.getFrom()));
			cell.setCellStyle(defaultDataCellStyle);

			// --- To
			cellNumber++;
			cell = row.createCell(cellNumber);
			cell.setCellValue(new HSSFRichTextString(email.getTo()));
			cell.setCellStyle(defaultDataCellStyle);

			// --- Cc
			cellNumber++;
			cell = row.createCell(cellNumber);
			cell.setCellValue(new HSSFRichTextString(email.getCc() == null ? ""
					: email.getCc()));
			cell.setCellStyle(defaultDataCellStyle);

			// --- Subject
			cellNumber++;
			cell = row.createCell(cellNumber);
			cell.setCellValue(new HSSFRichTextString(email.getSubject()));
			cell.setCellStyle(defaultDataCellStyle);

			// --- State
			cellNumber++;
			cell = row.createCell(cellNumber);
			cell.setCellValue(new HSSFRichTextString(email.getState()
					.getShortDescription()));
			cell.setCellStyle(defaultDataCellStyle);

			// --- Error reason
			cellNumber++;
			cell = row.createCell(cellNumber);
			cell.setCellValue(new HSSFRichTextString(
					email.getErrorReason() == null ? "" : email
							.getCustomerErrorReason()));
			cell.setCellStyle(defaultDataCellStyle);

			// --- Created Date
			cellNumber++;
			cell = row.createCell(cellNumber);
			Calendar calendar = Calendar.getInstance();
			calendar.setTimeInMillis(email.getCreatedDate().getTime());
			cell.setCellValue(calendar);
			cell.setCellStyle(dateTimeCellStyle);

			// --- Sent Date
			cellNumber++;
			cell = row.createCell(cellNumber);
			if (email.getSentDate() != null) {
				calendar = Calendar.getInstance();
				calendar.setTimeInMillis(email.getSentDate().getTime());
				cell.setCellValue(calendar);
			}
			cell.setCellStyle(dateTimeCellStyle);
		}

		return workbook;
	}

	public static void enqueueForDelivery(Email email) {
		if (sendImmediately) {
			sendAndStoreEmail(email);
		} else {
			email.setState(Email.State.READY);
			saveEmail(email);
		}
	}

	public static void storeAsFailed(Email email) {
		email.setState(Email.State.ERROR);
		saveEmail(email);
	}

	public static void setSendImmediately(boolean sendImmediately) {
		EmailService.sendImmediately = sendImmediately;
	}

	public static boolean isSendImmediately() {
		return sendImmediately;
	}
}
