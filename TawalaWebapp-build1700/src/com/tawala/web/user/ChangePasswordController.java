package com.tawala.web.user;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class ChangePasswordController extends SimpleFormController {
    public ChangePasswordController() {
        setCommandName("passwordChangeForm");
        setFormView("user.password.change");
        setSuccessView("user.password.change");
    }

    protected Object formBackingObject(HttpServletRequest request)
            throws Exception {
        return new ChangePasswordForm();
    }

    protected void onBindAndValidate(HttpServletRequest request,
            Object command, BindException errors) throws Exception {

        ChangePasswordForm form = (ChangePasswordForm) command;
        User user = UserInfoPreparationInterceptor.getSessionUser(request);
        
        if(! user.checkPassword(form.getOldPassword())) {
            errors.rejectValue("oldPassword", "password.doesnt.match");
        }
        
        if(! form.getPassword().equals(form.getRepeatedPassword())) { 
            errors.rejectValue("password", "user.password.mismatch" );
        }
    }

    protected ModelAndView onSubmit(HttpServletRequest request,
            HttpServletResponse response, Object command, BindException errors)
            throws Exception {

        User user = UserInfoPreparationInterceptor.getSessionUser(request);

        ChangePasswordForm form = (ChangePasswordForm) command;
        
        user.setPassword(form.getPassword());
        user.setRequirePasswordReset(false);

        WorldInitializer.getDefaultWorld().domain().users().addOrSave(
               user);

        Log.info(this, "Successfully changed password for user '" + user.getId()
                + "'.");

        ModelAndView result = super
                .onSubmit(request, response, command, errors);

        result.addObject("passwordSuccessfullyChanged", true);

        return result;
    }
}
