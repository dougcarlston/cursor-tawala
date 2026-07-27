package com.tawala.web.library;

import java.util.HashMap;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.validation.Validator;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.project.library.LibraryChangeEvent;
import com.tawala.project.library.ProjectChangeEvent;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class RevertEventController extends SimpleFormController {
    public static final String PARAMETER_EVENT_ID = "eventId";

    public RevertEventController() {
        setCommandName("form");
        setCommandClass(Form.class);
        setFormView("library.revert.event");
        setBindOnNewForm(true);
        setValidator(new FormValidator());
    }

    @Override
    protected ModelAndView onSubmit(HttpServletRequest request,
            HttpServletResponse response, Object command, BindException errors)
            throws Exception {
        Form form = (Form) command;

        LibraryChangeEvent event = ProjectLibraryService.findEventById(form
                .getEventId());

        event.revertChanges(UserInfoPreparationInterceptor
                .getSessionUser(request));

        return LibraryNavigation.navigateToSearchPage(response);
    }

    @Override
    protected Map referenceData(HttpServletRequest request, Object command,
            Errors errors) throws Exception {
        Form form = (Form) command;

        Map<String, Object> result = new HashMap<String, Object>();
        LibraryChangeEvent event = ProjectLibraryService.findEventById(form.getEventId());
        result.put("event", event);
        if(event.isProjectRelated()) {
            result.put("project", ProjectLibraryService.findProjectById(((ProjectChangeEvent)event).getProjectId()));
        }
        
        return result;
    }

    public static class Form {
        private boolean confirmRevert;
        private long eventId;

        public long getEventId() {
            return eventId;
        }

        public void setEventId(long eventId) {
            this.eventId = eventId;
        }

        public boolean isConfirmRevert() {
            return confirmRevert;
        }

        public void setConfirmRevert(boolean confirmRestore) {
            this.confirmRevert = confirmRestore;
        }
    }

    public static class FormValidator implements Validator {

        public boolean supports(Class clazz) {
            return Form.class.equals(clazz);
        }

        public void validate(Object obj, Errors errors) {
            Form form = (Form) obj;
            if (!form.isConfirmRevert()) {
                errors.rejectValue("confirmRevert",
                        "submittedproject.revert.event.must.confirm");
            }
        }
    }

}
