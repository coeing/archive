package models.user.item;

import javax.persistence.CascadeType;
import javax.persistence.Entity;
import javax.persistence.ManyToOne;
import javax.persistence.OneToOne;

import org.apache.commons.lang.builder.ReflectionToStringBuilder;

import models.BaseModel;
import models.article.Article;
import models.user.User;

@Entity
public class WantedItem extends UserItem {
    public WantedItem(Article article) {
	super(article);
    }
}
