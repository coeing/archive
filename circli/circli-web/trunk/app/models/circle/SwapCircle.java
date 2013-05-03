package models.circle;

import play.*;
import play.db.jpa.*;

import javax.persistence.*;

import org.apache.commons.lang.builder.ReflectionToStringBuilder;
import org.apache.commons.lang.builder.ToStringBuilder;

import models.BaseModel;
import models.user.User;
import models.user.item.OwnedItem;

import java.util.*;

@Entity
public class SwapCircle extends BaseModel {
    @OneToMany(cascade = CascadeType.ALL, mappedBy = "circle", fetch = FetchType.EAGER)
    private Set<SwapCircleStep> steps = new HashSet<SwapCircleStep>();

    /**
     * Default constructor.
     */
    public SwapCircle() {
	// default c'tor
    }

    public void addStep(SwapCircleStep step) {
	getSteps().add(step);
	step.setCircle(this);
    }

    public Set<SwapCircleStep> getSteps() {
	return steps;
    }

    public boolean hasOwnedItemWithUser(User user) {
	for (SwapCircleStep step : getSteps()) {
	    if (step.getOwnedItem().getUser().equals(user)) {
		return true;
	    }
	}
	return false;
    }

    private boolean hasStepWithReceivingUser(User user) {
	for (SwapCircleStep step : getSteps()) {
	    if (step.getWantedItem().getUser().equals(user)) {
		return true;
	    }
	}
	return false;
    }

    /**
     * Checks whether the circle is complete which means that every step has a corresponding that in
     * which the receiving user matches the giving one. An empty circle is per definition not
     * complete.
     * 
     * @return true if the the circle is complete.
     */
    public boolean isComplete() {
	if (getSteps().isEmpty()) {
	    return false;
	}
	boolean isComplete = true;
	for (SwapCircleStep step : getSteps()) {
	    isComplete = isComplete && hasStepWithReceivingUser(step.getOwnedItem().getUser());
	}
	return isComplete;
    }

    public void setSteps(Set<SwapCircleStep> circleElements) {
	this.steps = circleElements;
    }

    @Override
    public String toInspectString() {
	return String.format("Circle of size %d. Steps: %s", getSteps().size(), getSteps());
    }

    @Override
    public String toString() {
	return new ToStringBuilder(this).append(steps).toString();
    }
}