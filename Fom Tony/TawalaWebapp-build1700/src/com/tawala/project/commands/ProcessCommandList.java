package com.tawala.project.commands;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Iterator;
import java.util.List;
import java.util.ListIterator;
import java.util.Set;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;

public class ProcessCommandList extends ProcessCommand implements List<ProcessCommand> {
    private static final Factory<ProcessCommand> FACTORY = new Factory<ProcessCommand>();

    static {
        FACTORY.register("show", "document", ShowDocument.class);
        FACTORY.register("show", "form", ShowForm.class);
        FACTORY.register("show", Show.class);
        FACTORY.register("edit", EditRecord.class);
        FACTORY.register("if", If.class);
        FACTORY.register("send", Send.class);
        FACTORY.register("set", SetCommand.class);
        FACTORY.register("append", Append.class);
        FACTORY.register("addTo", MathCommand.AddTo.class);
        FACTORY.register("subtractFrom", MathCommand.SubtractFrom.class);
        FACTORY.register("multiplyBy", MathCommand.MultiplyBy.class);
        FACTORY.register("divideBy", MathCommand.DivideBy.class);
        FACTORY.register("get", Get.class);
        FACTORY.register("delete", Delete.class);
        FACTORY.register("foreach", ForEach.class);
        FACTORY.register("forEachMc", ForEachMc.class);
        FACTORY.ignore("comment");
    }

    private final List<ProcessCommand> contents = new ArrayList<ProcessCommand>();

    public ProcessCommandList() {
    }

    public ProcessCommandList(ConfigElement config) {
        super(config);
        contents.addAll(FACTORY.makeChildren(config));
    }

    public ExecutionResult execute(ExecutionContext context) {
        //--- Don't use ExecutionResult.NULL here - it's supposed to be unmodifiable.
        ExecutionResult finalResult = new ExecutionResult();
        for (ProcessCommand command : contents) {
            ExecutionResult result = command.execute(context);
            finalResult = finalResult.add(result);
            if (result.stopImmediately()) 
                return finalResult;
        }
        return finalResult;
    }

    public List<ProcessCommand> getContents() {
        return contents;
    }

    public int size() {
        return contents.size();
    }

    public boolean isEmpty() {
        return contents.isEmpty();
    }

    public boolean contains(Object o) {
        return contents.contains(o);
    }

    public Iterator<ProcessCommand> iterator() {
        return contents.iterator();
    }

    public Object[] toArray() {
        return contents.toArray();
    }

    public <T> T[] toArray(T[] a) {
        return contents.toArray(a);
    }

    public boolean add(ProcessCommand o) {
        return contents.add(o);
    }

    public boolean remove(Object o) {
        return contents.remove(o);
    }

    public boolean containsAll(Collection<?> c) {
        return contents.containsAll(c);
    }

    public boolean addAll(Collection<? extends ProcessCommand> c) {
        return contents.addAll(c);
    }

    public boolean addAll(int index, Collection<? extends ProcessCommand> c) {
        return contents.addAll(index, c);
    }

    public boolean removeAll(Collection<?> c) {
        return contents.removeAll(c);
    }

    public boolean retainAll(Collection<?> c) {
        return contents.retainAll(c);
    }

    public void clear() {
        contents.clear();
    }

    public boolean equals(Object o) {
        return contents.equals(o);
    }

    public int hashCode() {
        return contents.hashCode();
    }

    public ProcessCommand get(int index) {
        return contents.get(index);
    }

    public ProcessCommand set(int index, ProcessCommand element) {
        return contents.set(index, element);
    }

    public void add(int index, ProcessCommand element) {
        contents.add(index, element);
    }

    public ProcessCommand remove(int index) {
        return contents.remove(index);
    }

    public int indexOf(Object o) {
        return contents.indexOf(o);
    }

    public int lastIndexOf(Object o) {
        return contents.lastIndexOf(o);
    }

    public ListIterator<ProcessCommand> listIterator() {
        return contents.listIterator();
    }

    public ListIterator<ProcessCommand> listIterator(int index) {
        return contents.listIterator(index);
    }

    public List<ProcessCommand> subList(int fromIndex, int toIndex) {
        return contents.subList(fromIndex, toIndex);
    }

    @Override
    public void addFormsNamesCanNavigateTo(Set<String> result) {
    	for (ProcessCommand command : this) {
			command.addFormsNamesCanNavigateTo(result);
		}
    }
}
