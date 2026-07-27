package com.tawala.web.user;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.web.controller.UserAccessTicketInterceptor;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class LogoutController implements Controller {

    public ModelAndView handleRequest(HttpServletRequest request,
            HttpServletResponse response) throws Exception {

        User user = UserInfoPreparationInterceptor.getSessionUser(request);
        if(user != null) {
            Log.info(this, "successful logout for " + user.getId());
            EventService.createEvent(new Event("Logout", request));
        }

        handleLogout(request, response);

        response.sendRedirect(WellKnown.urls.getHome());
        
        return null;
    }

    /**
     * @param request
     * @param response
     */
    public static void handleLogout(HttpServletRequest request, HttpServletResponse response) {
        HttpSession session = request.getSession(false);
        if(session != null) {
            session.invalidate();
        }
        
        response.addCookie(LoginController.removeForumIntegrationCookie());
        UserAccessTicket ticket = UserAccessTicketInterceptor.getUserAccessTicket(request);
        if(ticket != null) {
        	response.addCookie(LoginController.removeUserAccessCookie());
        	UsersHibernateImpl.deleteAccessTicket(ticket);
        }
    }

}
