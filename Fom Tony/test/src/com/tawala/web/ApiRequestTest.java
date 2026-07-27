package com.tawala.web;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.TestCase;

public class ApiRequestTest extends TestCase {
    public void testBasics() {
        ConfigElement xml = parseConfig("    <request type=\"queryDeployments\" protocol=\"1.0\">\n" +
                "        <credentials user=\"william\" />\n" +
                "    </request>\n");
        ApiRequest request = new ApiRequest(xml);
        assertEquals("queryDeployments", request.getType());
        assertEquals("william", request.getUserId());
    }
}
