package com.tawala.domain;

import java.lang.annotation.Annotation;
import java.lang.reflect.Field;

import javax.persistence.Column;
import javax.persistence.Embeddable;
import javax.persistence.Entity;

import com.tawala.payment.ProjectInvoice;
import com.tawala.project.UserProject;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.theme.UserUploadedFile;
import com.tawala.web.user.UserAccessTicket;

//--- TODO: caching of metadata on application startup.
public class DomainMetadata {
	public static final DomainMetadata instance = new DomainMetadata();

	private DomainMetadata() {
		// --- To prevent multiple instance creation.
	}

	public int getUserFileIdMaxLength() {
		return detectFieldLengthFromHibernateAnnotation(UserUploadedFile.class,
				"id");
	}

	public int getLibraryProjectShortDescriptionMaxLength() {
		return detectFieldLengthFromHibernateAnnotation(LibraryProject.class,
				"shortDescription");
	}

	public int getLibraryProjectNameMaxLength() {
		return detectFieldLengthFromHibernateAnnotation(LibraryProject.class,
				"name");
	}

	public int getUserProjectNameMaxLength() {
		return detectFieldLengthFromHibernateAnnotation(UserProject.class,
				"name");
	}

	public int getProjectUniqueTokenLength() {
		return detectFieldLengthFromHibernateAnnotation(UserProject.class,
				"uniqueRandomId") - 5;
	}

	public int getProjectInvoiceIdLength() {
		return detectFieldLengthFromHibernateAnnotation(ProjectInvoice.class,
				"id");
	}

	public int getUserNameMaxLength() {
		return detectFieldLengthFromHibernateAnnotation(User.class, "userName");
	}

	public int getUserEmailAddressMaxLength() {
		return detectFieldLengthFromHibernateAnnotation(EmailAddress.class,
				"address");
	}

	public int getUserAccessTicketLength() {
		return detectFieldLengthFromHibernateAnnotation(UserAccessTicket.class,
				"accessToken");
	}

	@SuppressWarnings("unchecked")
	private static int detectFieldLengthFromHibernateAnnotation(Class clazz,
			String fieldName) {
		if (!(clazz.isAnnotationPresent(Entity.class) || clazz
				.isAnnotationPresent(Embeddable.class))) {
			throw new IllegalArgumentException(clazz + " must be annotated as "
					+ Entity.class + " or " + Embeddable.class);
		}

		try {
			Field field = clazz.getDeclaredField(fieldName);

			Annotation[] annotations = field.getAnnotations();
			for (int i = 0; i < annotations.length; i++) {
				Annotation annotation = annotations[i];
				if (annotation.annotationType() == Column.class) {
					Column column = (Column) annotation;
					return column.length();
				}
			}

			throw new IllegalArgumentException(clazz + "." + fieldName
					+ " is not annotated as " + Column.class);
		} catch (SecurityException e) {
			throw new IllegalArgumentException(clazz + "." + fieldName
					+ " is not accessible:", e);
		} catch (NoSuchFieldException e) {
			throw new IllegalArgumentException(clazz + "." + fieldName
					+ " does not exist:", e);
		}
	}
}
