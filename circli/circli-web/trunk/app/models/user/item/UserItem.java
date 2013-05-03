package models.user.item;

import play.*;
import play.data.validation.Required;
import play.db.jpa.*;

import javax.persistence.*;

import org.apache.commons.lang.builder.ReflectionToStringBuilder;

import models.BaseModel;
import models.article.Article;
import models.user.User;

import java.util.*;

@MappedSuperclass
public abstract class UserItem extends BaseModel {
    public UserItem(Article article) {
	this.article = article;
    }

    @ManyToOne(optional = false, cascade = CascadeType.ALL)
    private User user;

    @ManyToOne(optional = false)
    private Article article;

    public User getUser() {
	return user;
    }

    public void setUser(User user) {
	this.user = user;
    }

    public Article getArticle() {
	return article;
    }

    public void setArticle(Article article) {
	this.article = article;
    }
    
    @Override
    public String toString() {
	return ReflectionToStringBuilder.toString(this);
    }
}
