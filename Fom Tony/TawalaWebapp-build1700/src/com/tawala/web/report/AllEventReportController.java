package com.tawala.web.report;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Timestamp;
import java.text.SimpleDateFormat;
import java.util.Date;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFCellStyle;
import org.apache.poi.hssf.usermodel.HSSFDataFormat;
import org.apache.poi.hssf.usermodel.HSSFFont;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.apache.poi.hssf.util.HSSFColor;
import org.apache.poi.hssf.util.Region;
import org.hibernate.HibernateException;
import org.hibernate.Session;
import org.springframework.orm.hibernate3.HibernateCallback;
import org.springframework.validation.BindException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.hibernate.TawalaSessionFactory;

public class AllEventReportController extends SimpleFormController {
	public AllEventReportController() {
		setCommandClass(Form.class);
		setCommandName("form");
		setFormView("report.all.events");
	}
	
	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		Form form = (Form) command;

		HSSFWorkbook workbook = generateReport(form.getStartDate(), form
				.getEndDate());

		response.setContentType("application/vnd.ms-excel");
		response.setHeader("Content-Disposition", "attachment; filename=\""
				+ "Tawala Event Report (" + form.getStartDate() + " to "
				+ form.getEndDate() + ").xls\";");
		workbook.write(response.getOutputStream());

		return null;
	}

	private static HSSFWorkbook generateReport(final String startDateParameter,
			final String endDateParameter) {
		return (HSSFWorkbook) TawalaSessionFactory.MAIN.getHibernateTemplate()
				.execute(new HibernateCallback() {
					@SuppressWarnings("unchecked")
					public Object doInHibernate(Session session)
							throws HibernateException, SQLException {

						final HSSFWorkbook workbook = new HSSFWorkbook();
						final HSSFSheet sheet = workbook.createSheet("Report");
						sheet.setDisplayGridlines(false);

						HSSFCellStyle defaultDataCellStyle = workbook
								.createCellStyle();
						defaultDataCellStyle
								.setBorderBottom(HSSFCellStyle.BORDER_DOTTED);
						defaultDataCellStyle
								.setBottomBorderColor(HSSFColor.GREY_25_PERCENT.index);
						defaultDataCellStyle
								.setBorderLeft(HSSFCellStyle.BORDER_DOTTED);
						defaultDataCellStyle
								.setLeftBorderColor(HSSFColor.GREY_25_PERCENT.index);

						HSSFCellStyle dateTimeCellStyle = workbook
								.createCellStyle();
						dateTimeCellStyle
								.setBorderBottom(HSSFCellStyle.BORDER_DOTTED);
						dateTimeCellStyle
								.setBottomBorderColor(HSSFColor.GREY_25_PERCENT.index);
						dateTimeCellStyle
								.setBorderLeft(HSSFCellStyle.BORDER_DOTTED);
						dateTimeCellStyle
								.setLeftBorderColor(HSSFColor.GREY_25_PERCENT.index);
						dateTimeCellStyle.setDataFormat(HSSFDataFormat
								.getBuiltinFormat("m/d/yy h:mm"));

						HSSFCellStyle dateCellStyle = workbook
								.createCellStyle();
						dateCellStyle
								.setBorderBottom(HSSFCellStyle.BORDER_DOTTED);
						dateCellStyle
								.setBottomBorderColor(HSSFColor.GREY_25_PERCENT.index);
						dateCellStyle
								.setBorderLeft(HSSFCellStyle.BORDER_DOTTED);
						dateCellStyle
								.setLeftBorderColor(HSSFColor.GREY_25_PERCENT.index);
						dateCellStyle.setDataFormat(HSSFDataFormat
								.getBuiltinFormat("m/d/yy"));

						int rowCount = -1;

						rowCount = addTitleAndHeaders(workbook, sheet,
								rowCount, startDateParameter, endDateParameter);

						Statement statement = session.connection()
								.createStatement();
						try {
							String query = "select e.visitor_id, e.user_id, u.user_name, e.type, e.param1, e.created_dt "
									+ " from event e left outer join users u using(user_id)"
									+ " where date_trunc('day', e.created_dt) between date '"
									+ startDateParameter
									+ "' and date '"
									+ endDateParameter
									+ "'"
									+ " order by created_dt, user_name, visitor_id;";

							statement.executeQuery(query);
							ResultSet result = statement.getResultSet();
							try {
								while (result.next()) {
									short cellNumber = 0;
									HSSFRow row = sheet.createRow(++rowCount);

									// --- Visitor Id
									HSSFCell cell = row.createCell(cellNumber);
									cell.setCellValue(result
											.getString(cellNumber + 1));
									cell.setCellStyle(defaultDataCellStyle);

									// --- User Id
									cellNumber++;
									cell = row.createCell(cellNumber);
									cell.setCellValue(result
											.getString(cellNumber + 1));
									cell.setCellStyle(dateCellStyle);

									// --- User Name
									cellNumber++;
									cell = row.createCell(cellNumber);
									cell.setCellValue(result
											.getString(cellNumber + 1));
									cell.setCellStyle(defaultDataCellStyle);

									// --- Type
									cellNumber++;
									cell = row.createCell(cellNumber);
									cell.setCellValue(result
											.getString(cellNumber + 1));
									cell.setCellStyle(dateTimeCellStyle);

									// --- Param1
									cellNumber++;
									cell = row.createCell(cellNumber);
									cell.setCellValue(result
											.getString(cellNumber + 1));
									cell.setCellStyle(defaultDataCellStyle);

									// --- Date created
									cellNumber++;
									cell = row.createCell(cellNumber);
									Timestamp eventDate = result
											.getTimestamp(cellNumber + 1);
									cell.setCellValue(eventDate);
									cell.setCellStyle(dateTimeCellStyle);
								}
							} finally {
								result.close();
							}
						} finally {
							statement.close();
						}

						return workbook;
					}

					private int addTitleAndHeaders(final HSSFWorkbook workbook,
							final HSSFSheet sheet, int rowCount,
							String startDateParameter, String endDateParameter) {
						final short totalCellCount = 5; // --- Actually, -1.

						// --- Add Title
						HSSFCellStyle titleCellStyle = workbook
								.createCellStyle();
						titleCellStyle.setWrapText(true);
						titleCellStyle.setAlignment(HSSFCellStyle.ALIGN_CENTER);
						titleCellStyle
								.setVerticalAlignment(HSSFCellStyle.VERTICAL_CENTER);

						HSSFFont titleFont = workbook.createFont();
						titleFont.setBoldweight(HSSFFont.BOLDWEIGHT_BOLD);
						titleFont.setFontHeightInPoints((short) 12);
						titleFont.setItalic(true);
						titleCellStyle.setFont(titleFont);

						HSSFRow row = sheet.createRow(++rowCount);
						HSSFCell cell = row.createCell((short) 0);
						cell.setCellValue("Event Report (" + startDateParameter
								+ " to " + endDateParameter + ")");
						cell.setCellStyle(titleCellStyle);
						row.setHeightInPoints(30);

						sheet.addMergedRegion(new Region(rowCount, (short) 0,
								rowCount, totalCellCount));

						HSSFCellStyle reportDescriptionCellStyle = workbook
								.createCellStyle();
						reportDescriptionCellStyle.setWrapText(true);
						reportDescriptionCellStyle
								.setAlignment(HSSFCellStyle.ALIGN_RIGHT);
						reportDescriptionCellStyle
								.setVerticalAlignment(HSSFCellStyle.VERTICAL_CENTER);

						HSSFFont reportDescriptionFont = workbook.createFont();
						reportDescriptionFont.setFontHeightInPoints((short) 10);
						reportDescriptionFont.setItalic(true);
						reportDescriptionFont
								.setColor(HSSFColor.GREY_50_PERCENT.index);
						reportDescriptionCellStyle
								.setFont(reportDescriptionFont);

						row = sheet.createRow(++rowCount);
						cell = row.createCell((short) 0);
						cell.setCellStyle(reportDescriptionCellStyle);
						cell.setCellValue("Generated on "
								+ new SimpleDateFormat("EEEE MMM d, yyyy")
										.format(new Date()));
						row.setHeightInPoints(20);
						sheet.addMergedRegion(new Region(rowCount, (short) 0,
								rowCount, totalCellCount));

						// --- Add header
						HSSFCellStyle headerCellStyle = workbook
								.createCellStyle();
						headerCellStyle
								.setBorderBottom(HSSFCellStyle.BORDER_DOUBLE);
						headerCellStyle
								.setBorderTop(HSSFCellStyle.BORDER_DOUBLE);
						headerCellStyle
								.setBorderLeft(HSSFCellStyle.BORDER_THIN);
						headerCellStyle
								.setAlignment(HSSFCellStyle.ALIGN_CENTER);
						headerCellStyle.setWrapText(true);

						HSSFFont headerFont = workbook.createFont();
						headerFont.setBoldweight(HSSFFont.BOLDWEIGHT_BOLD);
						headerFont.setFontHeightInPoints((short) 10);
						headerCellStyle.setFont(headerFont);

						short cellNumber = 0;
						HSSFRow headerRow = sheet.createRow(++rowCount);
						cell = headerRow.createCell(cellNumber);
						cell.setCellStyle(headerCellStyle);
						cell.setCellValue("Visitor Id");
						sheet.setColumnWidth(cellNumber, (short) (15 * 256));

						cellNumber++;
						cell = headerRow.createCell(cellNumber);
						cell.setCellStyle(headerCellStyle);
						cell.setCellValue("User Id");
						sheet.setColumnWidth(cellNumber, (short) (15 * 256));

						cellNumber++;
						cell = headerRow.createCell(cellNumber);
						cell.setCellStyle(headerCellStyle);
						cell.setCellValue("User Name");
						sheet.setColumnWidth(cellNumber, (short) (20 * 256));

						cellNumber++;
						cell = headerRow.createCell(cellNumber);
						cell.setCellStyle(headerCellStyle);
						cell.setCellValue("Type");
						sheet.setColumnWidth(cellNumber, (short) (30 * 256));

						cellNumber++;
						cell = headerRow.createCell(cellNumber);
						cell.setCellStyle(headerCellStyle);
						cell.setCellValue("Details");
						sheet.setColumnWidth(cellNumber, (short) (30 * 256));

						cellNumber++;
						cell = headerRow.createCell(cellNumber);
						cell.setCellStyle(headerCellStyle);
						cell.setCellValue("Date");
						sheet.setColumnWidth(cellNumber, (short) (20 * 256));

						return rowCount;
					}

				}, true);
	}

	public static class Form {
		private String startDate;
		private String endDate;

		public String getEndDate() {
			return endDate;
		}

		public void setEndDate(String endDate) {
			this.endDate = endDate;
		}

		public String getStartDate() {
			return startDate;
		}

		public void setStartDate(String startDate) {
			this.startDate = startDate;
		}
	}
}
