package com.tawala.web;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;

import org.springframework.orm.hibernate3.HibernateTemplate;

import com.scissor.Log;
import com.tawala.domain.EmailAddress;
import com.tawala.domain.Role;
import com.tawala.domain.User;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.jbpm.JbpmService;
import com.tawala.project.library.Constants;
import com.tawala.project.library.ProjectLibrary;

//--- When renaming make sure to update web.xml
public class PredeterminedObjectInitializer extends HttpServlet {
	private static final long serialVersionUID = 1L;

	@Override
	public void init() throws ServletException {
		addAnonymousUser();
		addProjectLibraries();
		addUserRoles();
		updateSportsDashboardProcessDefinitionIfNeeded();
		super.init();
	}

	private void updateSportsDashboardProcessDefinitionIfNeeded() {
		JbpmService.deployTheLatestDefinitionIfNeeded(com.tawala.jbpm.sportsdashboards.Constants.PROCESS_NAME);
	}

	private void addUserRoles() {
		HibernateTemplate hibernateTemplate = TawalaSessionFactory.MAIN
				.getHibernateTemplate();
		for (Role role : Role.PREDEFINED_ROLES) {
			if (hibernateTemplate.get(Role.class, role.getRoleId()) == null) {
				hibernateTemplate.save(role);
			}
		}

	}

	private void addProjectLibraries() {
		for (ProjectLibrary projectLibrary : ProjectLibrary
				.getAllPredeterminedLibraries()) {
			if (TawalaSessionFactory.MAIN.getHibernateTemplate().get(
					ProjectLibrary.class, projectLibrary.getId()) == null) {
				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						projectLibrary);
			}
		}
	}

	private void addAnonymousUser() {
		User anonymousUser = WorldInitializer.getDefaultWorld().domain()
				.users().get(Constants.ANONYMOUS_USER_ID);
		if (anonymousUser == null) {
			anonymousUser = new User("anonymous", "Anonymous", "User",
					new EmailAddress("nobody@example.com"), "nopassword");
			anonymousUser.setSuspended(true);
			WorldInitializer.getDefaultWorld().domain().users().addOrSave(
					anonymousUser);

			TawalaSessionFactory.MAIN.getHibernateTemplate().bulkUpdate(
					"update " + User.class.getName()
							+ " user set user.databaseId="
							+ Constants.ANONYMOUS_USER_ID
							+ " where user.databaseId="
							+ anonymousUser.getDatabaseId());
			Log.info(this, "Created anonymous user.");
		}
	}
}
