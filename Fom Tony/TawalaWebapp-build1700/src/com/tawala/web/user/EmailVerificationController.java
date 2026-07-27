package com.tawala.web.user;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.web.WorldInitializer;

public class EmailVerificationController implements Controller {
    public final static String PARAMETER_ID = "id";
    public final static String PARAMETER_VALIDATION_TOKEN = "x";

    public ModelAndView handleRequest(HttpServletRequest request,
            HttpServletResponse response) throws Exception {
        String userId = request.getParameter(PARAMETER_ID);
        String validationToken = request
                .getParameter(PARAMETER_VALIDATION_TOKEN);

        if (userId == null || validationToken == null) {
            return prepareNavigationToErrorPage();
        }

        User user = WorldInitializer.getDefaultWorld().domain().users().get(userId);
        if (user == null) {
            return prepareNavigationToErrorPage();
        }

        if (!user.getStatus().hasEmailBeenValidated()
                && !user.getEmailValidationToken().equals(validationToken)) {
            Log.warn(this, "Email validation token of the user"
                    + user.getEmailValidationToken()
                    + ") didn't match the token submitted through the link ("
                    + validationToken + ")");
            return prepareNavigationToErrorPage();
        }

        WorldInitializer.getDefaultWorld().domain().users().onUserEmailValidation(user);
        WorldInitializer.getDefaultWorld().domain().users().onUserApproval(user);
        
        ModelAndView result = new ModelAndView("registration.email.verified");
        result.addObject("confirmedUser", user);
        return result;
    }

    private static ModelAndView prepareNavigationToErrorPage() {
        return new ModelAndView("registration.email.verification.error");
    }
}
