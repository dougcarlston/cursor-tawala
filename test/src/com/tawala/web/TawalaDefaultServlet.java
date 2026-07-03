package com.tawala.web;

import java.util.Hashtable;

import javax.naming.InitialContext;
import javax.servlet.ServletException;

import org.apache.catalina.Globals;
import org.apache.catalina.servlets.DefaultServlet;
import org.apache.naming.resources.FileDirContext;
import org.apache.naming.resources.ProxyDirContext;

import com.scissor.Log;

public class TawalaDefaultServlet extends DefaultServlet {
    private static final long serialVersionUID = 1L;

    /*
     * (non-Javadoc)
     * 
     * @see org.apache.catalina.servlets.DefaultServlet#init()
     */
    @Override
    public void init() throws ServletException {
        try {
            if (getServletContext().getAttribute(Globals.RESOURCES_ATTR) == null
                    || new InitialContext().lookup(RESOURCES_JNDI_NAME) == null) {

                Hashtable<String, String> env = new Hashtable<String, String>();
                env.put(ProxyDirContext.CONTEXT, "/");

                FileDirContext dirContext = new FileDirContext();
                dirContext.setDocBase("web");
                
                getServletContext().setAttribute(Globals.RESOURCES_ATTR,
                        new ProxyDirContext(env, dirContext));

            }
        } catch (Exception e) {
            Log.error(this, "Error initializing " + this, e);
        }

        super.init();
    }

}
