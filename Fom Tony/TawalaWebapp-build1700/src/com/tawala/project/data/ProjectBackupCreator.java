package com.tawala.project.data;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Properties;
import java.util.regex.Pattern;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;
import java.util.zip.ZipOutputStream;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;

public class ProjectBackupCreator {
	private static final String ENTRY_NAME_PROJECT_FILE = "project.tawala";

	private static final String ENTRY_NAME_DATA_FILE = "data.xls";

	private static final String ENTRY_NAME_LINKS_PROPERTIES = "links.properties";

	private static final String ENTRY_NAME_PROJECT_PROPERTIES = "project.properties";

	private static final String ENTRY_NAME_BACKUP_PROPERTIES = "backup.properties";

	public static final String CURRENT_VERSION = "1.0";

	public static final String PROPERTY_BACKUP_VERSION_NUMBER = "backup.version";
	public static final String PROPERTY_PROJECT_ID = "project.id";
	public static final String PROPERTY_PROJECT_NAME = "project.name";
	public static final String PROPERTY_PROJECT_VERSION_NUMBER = "project.version";
	public static final String PROPERTY_PROJECT_UNIQUE_ID = "project.unique.random.id";

	public static ExportStatistics createBackup(final UserProject userProject,
			OutputStream outputStream) throws IOException {
		Properties backupInfo = new Properties();
		backupInfo.setProperty(PROPERTY_BACKUP_VERSION_NUMBER, CURRENT_VERSION);
		backupInfo.setProperty(PROPERTY_PROJECT_ID, Long.toString(userProject
				.getId()));
		backupInfo.setProperty(PROPERTY_PROJECT_NAME, userProject.getName());
		backupInfo.setProperty(PROPERTY_PROJECT_VERSION_NUMBER, Integer
				.toString(userProject.getDeployedVersion().getVersionNumber()));
		backupInfo.setProperty(PROPERTY_PROJECT_UNIQUE_ID, userProject
				.getUniqueRandomId());

		ZipOutputStream zip = new ZipOutputStream(outputStream);

		// --- Backup Info
		ZipEntry backupInfoEntry = new ZipEntry(ENTRY_NAME_BACKUP_PROPERTIES);
		backupInfoEntry.setMethod(ZipEntry.DEFLATED);
		zip.putNextEntry(backupInfoEntry);

		backupInfo.store(zip, "Backup Info");

		// --- Project File
		ZipEntry projectFileEntry = new ZipEntry(ENTRY_NAME_PROJECT_FILE);
		projectFileEntry.setMethod(ZipEntry.DEFLATED);
		zip.putNextEntry(projectFileEntry);
		zip
				.write(userProject.getProject().getProjectXmlDefinition()
						.getBytes());

		// --- Project Properties
		ZipEntry projectPropertiesEntry = new ZipEntry(
				ENTRY_NAME_PROJECT_PROPERTIES);
		projectPropertiesEntry.setMethod(ZipEntry.DEFLATED);
		zip.putNextEntry(projectPropertiesEntry);
		String propertiesAsString = userProject.getProject()
				.getPropertiesAsString();
		zip.write(propertiesAsString == null ? new byte[0] : propertiesAsString
				.getBytes());

		// --- Project Links
		ZipEntry projectLinksEntry = new ZipEntry(ENTRY_NAME_LINKS_PROPERTIES);
		projectLinksEntry.setMethod(ZipEntry.DEFLATED);
		zip.putNextEntry(projectLinksEntry);

		Properties properties = new Properties();
		List<LinkToUserProject> links = ProjectsHibernateImpl
				.getAllProjectLinks(userProject);
		for (LinkToUserProject link : links) {
			properties.put(link.getId(), link.isAuthenticated()
					+ "|"
					+ (link.getAuthenticationToken() == null ? "" : link
							.getAuthenticationToken()));
		}

		properties.store(zip, "Project Links");

		// --- Data
		ExportResult exportResult = ProjectToExcelExporter
				.exportAllProjectData(userProject);

		ZipEntry dataEntry = new ZipEntry(ENTRY_NAME_DATA_FILE);
		dataEntry.setMethod(ZipEntry.DEFLATED);
		zip.putNextEntry(dataEntry);
		exportResult.workbook.write(zip);

		zip.closeEntry();
		zip.close();
		
		return exportResult.statistics;
	}

	public static ProjectBackup recreateFromZip(InputStream stream)
			throws IOException {
		ProjectBackup result = new ProjectBackup();

		ZipInputStream backupStream = new ZipInputStream(stream);

		// --- Backup Properties
		ZipEntry zipEntry = backupStream.getNextEntry();
		if (!ENTRY_NAME_BACKUP_PROPERTIES.equals(zipEntry.getName())) {
			throw new IllegalStateException("Unexpected first entry name: "
					+ zipEntry.getName());
		}
		Properties backupProperties = new Properties();
		backupProperties.load(backupStream);

		if (!CURRENT_VERSION.equals(backupProperties
				.get(ProjectBackupCreator.PROPERTY_BACKUP_VERSION_NUMBER))) {
			throw new IllegalStateException(
					"Unable to recognize the backup version: " + "");
		}
		result.setBackupProperties(backupProperties);

		// --- Project File
		ZipEntry projectFile = backupStream.getNextEntry();
		if (!ENTRY_NAME_PROJECT_FILE.equals(projectFile.getName())) {
			throw new IllegalStateException("Expected project file, but got: "
					+ projectFile.getName());
		}
		result.setProject(new Project(new ConfigElement(backupStream)));

		// --- Project Properties
		ZipEntry projectPropertiesEntry = backupStream.getNextEntry();
		if (!ENTRY_NAME_PROJECT_PROPERTIES.equals(projectPropertiesEntry
				.getName())) {
			throw new IllegalStateException(
					"Expected project properties, but got: "
							+ projectPropertiesEntry.getName());
		}
		ByteArrayOutputStream data = new ByteArrayOutputStream();
		byte[] buffer = new byte[1024];
		int readBytes = 0;
		while ((readBytes = backupStream.read(buffer)) != -1) {
			data.write(buffer, 0, readBytes);
		}
		byte[] propertiesData = data.toByteArray();
		result.setProjectProperties(propertiesData == null
				|| propertiesData.length == 0 ? null : new String(
				propertiesData));

		// --- Project Links
		ZipEntry projectLinksEntry = backupStream.getNextEntry();
		if (!ENTRY_NAME_LINKS_PROPERTIES.equals(projectLinksEntry.getName())) {
			throw new IllegalStateException(
					"Expected link properties, but got: "
							+ projectLinksEntry.getName());
		}
		Properties linkProperties = new Properties();
		linkProperties.load(backupStream);

		List<LinkToUserProject> links = new ArrayList<LinkToUserProject>();
		Pattern parameterSplitter = Pattern.compile("\\|");
		for (Map.Entry<Object, Object> linkInfo : linkProperties.entrySet()) {
			String linkId = (String) linkInfo.getKey();
			String[] parameters = parameterSplitter.split(((String) linkInfo
					.getValue()), 2);
			boolean authenticated = Boolean.parseBoolean(parameters[0]);
			String authenticationToken = parameters[1].length() > 0 ? parameters[1]
					: null;

			links.add(new LinkToUserProject(linkId, authenticated,
					authenticationToken));
		}

		result.setLinks(links);

		// --- Project Data
		ZipEntry dataEntry = backupStream.getNextEntry();
		if (!ENTRY_NAME_DATA_FILE.equals(dataEntry.getName())) {
			throw new IllegalStateException("Expected data file, but got: "
					+ dataEntry.getName());
		}
		result.setData(new HSSFWorkbook(backupStream));

		return result;
	}
}
