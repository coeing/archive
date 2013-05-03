package models.article;

import play.*;
import play.db.jpa.*;

import javax.persistence.*;

import models.BaseModel;
import models.user.item.UserItem;

import java.util.*;

@Entity
@Inheritance(strategy = InheritanceType.JOINED)
public abstract class Article extends BaseModel {
    @Column(nullable = false)
    protected String uniqueIdentifier;

    public String getUniqueIdentifier() {
	return uniqueIdentifier;
    }

    public void setUniqueIdentifier(String uniqueIdentifier) {
	this.uniqueIdentifier = uniqueIdentifier;
    }
}
