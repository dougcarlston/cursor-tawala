package com.tawala.project.commands;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;

public abstract class ConditionList extends BooleanExpression {

    private List<BooleanExpression> contents;
    
    protected ConditionList() {
    	contents = new ArrayList<BooleanExpression>();
    }

    protected ConditionList(ConfigElement config) {
        contents = BooleanExpression.FACTORY.makeChildren(config);
    }

    public boolean isTrue(ExecutionContext context) {
        Iterator<BooleanExpression> i = contents.iterator();
        if (!i.hasNext()) return false;
        boolean result = i.next().isTrue(context);
        while (i.hasNext()) {
            BooleanExpression condition = i.next();
            result = add(result, condition.isTrue(context));
        }
        return result;
    }

    protected abstract boolean add(boolean a, boolean b);
	protected abstract int getUniqueConditionId();
    
    public List<BooleanExpression> contents() {
    	return contents;
    }
    
    public void add(BooleanExpression condition) {
    	contents.add(condition);
    }
    
    @Override
    public int nonObjectIdBasedHashCode() {
    	int result = 0;
    	for (BooleanExpression condition : contents()) {
			result += condition.nonObjectIdBasedHashCode();
		}
    	return result + getUniqueConditionId();
    }
}
