package com.tawala.web.user;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.validation.Validator;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.web.WorldInitializer;

public class ResetPasswordController extends SimpleFormController {
    public ResetPasswordController() {
        setSessionForm(false);
        setCommandName("login");
        setFormView("login.view");
        setSuccessView("login.view");
        setValidator(new ResetPasswordValidator());
    }

    static class ResetPasswordValidator implements Validator {
        public boolean supports(Class clazz) {
            return clazz.equals(LoginForm.class);
        }

        public void validate(Object obj, Errors errors) {
            LoginForm form = (LoginForm) obj;

            User user = WorldInitializer.getDefaultWorld().domain().users().get(
                    form.getUserName());
            if (user == null) {
                errors.reject("reset.password.user.not.found");
            } else if (user.getEmail() == null) {
                errors.reject("reset.password.no.email");
            } else {
                form.setUser(user);
            }
        }
    }

    @Override
    protected ModelAndView onSubmit(HttpServletRequest request,
            HttpServletResponse response, Object command, BindException errors)
            throws Exception {
        LoginForm form = (LoginForm) command;

        WorldInitializer.getDefaultWorld().domain().users().resetPassword(form.getUser());

        LogoutController.handleLogout(request, response);
        Log.info(this, "successful password reset for " + form.getUserName());
        
        ModelAndView result = new ModelAndView(getSuccessView());
        result.addObject(getCommandName(), form);
        result.addObject("passwordHasBeenReset", true);
        
        return result;
    }

    /*
     * (non-Javadoc)
     * 
     * @see org.springframework.web.servlet.mvc.AbstractFormController#formBackingObject(javax.servlet.http.HttpServletRequest)
     */
    @Override
    protected Object formBackingObject(HttpServletRequest request)
            throws Exception {
        return new LoginForm();
    }
}
