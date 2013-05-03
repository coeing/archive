package models.user;

import java.util.HashSet;
import java.util.Set;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.OneToMany;

import models.BaseModel;
import models.user.item.OwnedItem;
import models.user.item.WantedItem;

import org.apache.commons.lang.builder.ReflectionToStringBuilder;

import play.data.validation.Required;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

@Entity
public class User extends BaseModel {
    public static User createFromFacebookData(JsonObject data) {
	JsonElement idElement = data.get("id");
	JsonElement firstNameElement = data.get("first_name");
	JsonElement middleNameElement = data.get("middle_name");
	JsonElement lastNameElement = data.get("last_name");
	JsonElement emailElement = data.get("email");

	User user = new User();
	if (firstNameElement != null) {
	    user.firstName = firstNameElement.getAsString();
	}
	if (idElement != null) {
	    user.facebookId = idElement.getAsLong();
	}
	if (middleNameElement != null) {
	    user.middleName = middleNameElement.getAsString();
	}
	if (lastNameElement != null) {
	    user.lastName = lastNameElement.getAsString();
	}
	if (emailElement != null) {
	    user.email = emailElement.getAsString();
	}
	return user;
    }

    @Column(nullable = false)
    @Required
    public String firstName;

    private String middleName;

    @Required
    @Column(nullable = false)
    public String lastName;

    @Required
    @Column(nullable = false, unique = true)
    public String email;

    public Long facebookId;

    @OneToMany(mappedBy = "user", cascade = CascadeType.ALL)
    public Set<OwnedItem> ownedItems = new HashSet<OwnedItem>();

    @OneToMany(mappedBy = "user", cascade = CascadeType.ALL)
    public Set<WantedItem> wantedItems = new HashSet<WantedItem>();

    public void addOwnedItem(OwnedItem item) {
	ownedItems.add(item);
	item.setUser(this);
    }

    public void addWantedItem(WantedItem item) {
	wantedItems.add(item);
	item.setUser(this);
    }

    public String getFullName() {
	return String.format("%s %s %s", firstName, middleName == null ? "" : middleName, lastName);
    }

    @Override
    public String toInspectString() {
	return String.format("%s", getFullName());
    }

    @Override
    public String toString() {
	return ReflectionToStringBuilder.toString(this);
    }
}
