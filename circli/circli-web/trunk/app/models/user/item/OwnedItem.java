package models.user.item;

import javax.persistence.Entity;

import models.article.Article;

@Entity
public class OwnedItem extends UserItem {
    public OwnedItem(Article article) {
	super(article);
    }
}
