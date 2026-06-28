package com.tawala.jbpm;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.jbpm.graph.exe.ProcessInstance;
import org.jbpm.graph.exe.Token;

public class AggregateProcessStatistics {
	private Map<String, Integer> processStateCount;
	private int totalProcessCount;
	
	public AggregateProcessStatistics() {
		processStateCount = new HashMap<String, Integer>();
		totalProcessCount = 0;
	}
	
	@SuppressWarnings("unchecked")
	public void gatherInfo(ProcessInstance processInstance) {
		List<Token> tokens = processInstance.findAllTokens();
		for (Token token : tokens) {
			String stateDescription = token.getNode().getDescription();
			Integer previousCount = processStateCount.get(stateDescription);
			processStateCount.put(stateDescription , previousCount == null ? 1 : previousCount + 1);
		}
		
		totalProcessCount ++;
	}

	public Map<String, Integer> getProcessStateCount() {
		return processStateCount;
	}

	public int getTotalProcessCount() {
		return totalProcessCount;
	}
}
