package com.tawala.web.controller;

import java.util.LinkedHashMap;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;

import org.springframework.web.context.support.WebApplicationObjectSupport;
import org.springframework.web.servlet.HandlerExecutionChain;
import org.springframework.web.servlet.HandlerMapping;

abstract public class HandlerMappingByURLPrefix extends WebApplicationObjectSupport implements HandlerMapping {
    private Map<String, HandlerExecutionChain> mappings = new LinkedHashMap<String, HandlerExecutionChain>();

    public void addMapping(String prefix, HandlerExecutionChain handlerExecutionChain) {
    	mappings.put(prefix, handlerExecutionChain);
    }
    
    final public HandlerExecutionChain getHandler(HttpServletRequest request)
            throws Exception {
        String path = request.getRequestURI();
        for (Map.Entry<String, HandlerExecutionChain> mapping : mappings
                .entrySet()) {
            if (path.startsWith(mapping.getKey()))
                return mapping.getValue();
        }
        return null;
    }
}
