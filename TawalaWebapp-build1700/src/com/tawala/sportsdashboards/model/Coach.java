package com.tawala.sportsdashboards.model;

import org.json.JSONException;
import org.json.JSONObject;

import com.tawala.project.FormSubmission;
import com.tawala.project.Value;

public class Coach {
	private String id;
	private String firstName;
	private String lastName;
	private String middleName;
	private String birthDate;
	private Integer age;
	private String email;
	private String homePhone;
	private String workPhone;
	private String cellPhone;
	private String streetAddress;
	private String city;
	private String state;
	private String ZIP;
	private String statusId;
	private String statusMemo;
	private String affirmedAgreement;

	public Coach(FormSubmission submission) {
		this.id = submission.getValue("CoachId").toString();
		this.firstName = submission.getValue("FirstName").toString();
		this.lastName = submission.getValue("LastName").toString();
		this.middleName = submission.getValue("MiddleName").toString();
		this.birthDate = submission.getValue("BirthMonth").toString() + "/"
				+ submission.getValue("BirthDay").toString() + "/"
				+ submission.getValue("BirthYear").toString();
		Value ageValue = submission.getValue("Age");
		this.age = ageValue.containsNumber() ? ageValue.asNumber().intValue()
				: null;
		this.email = submission.getValue("Email").toString();
		this.homePhone = submission.getValue("HomePhone").toString();
		this.workPhone = submission.getValue("WorkPhone").toString();
		this.cellPhone = submission.getValue("CellPhone").toString();
		this.streetAddress = submission.getValue("Street").toString();
		this.city = submission.getValue("City").toString();
		this.state = submission.getValue("State").toString();
		this.ZIP = submission.getValue("Zip").toString();
		this.statusId = submission.getValue("StatusID").toString();
		this.statusMemo = submission.getValue("StatusMemo").toString();
		this.affirmedAgreement = submission.getValue("AffirmedAgreement")
				.toString();
	}

	public JSONObject toJSON() {
		JSONObject result = new JSONObject();
		try {
			result.put("id", id);
			result.put("firstName", firstName);
			result.put("lastName", this.lastName);
			result.put("middleName", this.middleName);
			result.put("birthDay", this.birthDate);
			if (age != null) {
				result.put("age", this.age.intValue());
			}
			result.put("email", this.email);
			result.put("homePhone", this.homePhone);
			result.put("workPhone", this.workPhone);
			result.put("cellPhone", this.cellPhone);
			result.put("streetAddress", this.streetAddress);
			result.put("city", this.city);
			result.put("state", this.state);
			result.put("zip", this.ZIP);
			result.put("statusId", this.statusId);
			result.put("statusMemo", this.statusMemo);
			result.put("affirmedAgreement", this.affirmedAgreement);
		} catch (JSONException e) {
			throw new IllegalStateException(
					"Unable to create JSON object from coach: ", e);
		}

		return result;
	}

	public String getId() {
		return id;
	}

	public void setId(String id) {
		this.id = id;
	}

	public String getFirstName() {
		return firstName;
	}

	public void setFirstName(String firstName) {
		this.firstName = firstName;
	}

	public String getLastName() {
		return lastName;
	}

	public void setLastName(String lastName) {
		this.lastName = lastName;
	}

	public String getMiddleName() {
		return middleName;
	}

	public void setMiddleName(String middleName) {
		this.middleName = middleName;
	}

	public String getBirthDate() {
		return birthDate;
	}

	public void setBirthDate(String birthDate) {
		this.birthDate = birthDate;
	}

	public int getAge() {
		return age;
	}

	public void setAge(int age) {
		this.age = age;
	}

	public String getEmail() {
		return email;
	}

	public void setEmail(String email) {
		this.email = email;
	}

	public String getHomePhone() {
		return homePhone;
	}

	public void setHomePhone(String homePhone) {
		this.homePhone = homePhone;
	}

	public String getWorkPhone() {
		return workPhone;
	}

	public void setWorkPhone(String workPhone) {
		this.workPhone = workPhone;
	}

	public String getCellPhone() {
		return cellPhone;
	}

	public void setCellPhone(String cellPhone) {
		this.cellPhone = cellPhone;
	}

	public String getStreetAddress() {
		return streetAddress;
	}

	public void setStreetAddress(String streetAddress) {
		this.streetAddress = streetAddress;
	}

	public String getCity() {
		return city;
	}

	public void setCity(String city) {
		this.city = city;
	}

	public String getState() {
		return state;
	}

	public void setState(String state) {
		this.state = state;
	}

	public String getZIP() {
		return ZIP;
	}

	public void setZIP(String zip) {
		ZIP = zip;
	}

	public String getStatusId() {
		return statusId;
	}

	public void setStatusId(String statusId) {
		this.statusId = statusId;
	}

	public String getStatusMemo() {
		return statusMemo;
	}

	public void setStatusMemo(String statusMemo) {
		this.statusMemo = statusMemo;
	}

	public String getAffirmedAgreement() {
		return affirmedAgreement;
	}

	public void setAffirmedAgreement(String affirmedAgreement) {
		this.affirmedAgreement = affirmedAgreement;
	}

}
