package models.article;

import javax.persistence.Column;
import javax.persistence.Entity;

import org.apache.commons.lang.builder.ReflectionToStringBuilder;
import org.apache.commons.lang.builder.ToStringBuilder;
import org.apache.commons.lang.builder.ToStringStyle;

@Entity
public class Book extends Article {
    @Column(nullable = false)
    private String author;

    @Column(nullable = false)
    private String title;

    public Book(String author, String title, String isbn) {
	this.author = author;
	this.title = title;
	setUniqueIdentifier(isbn);
    }

    public String getAuthor() {
	return author;
    }

    public String getIsbn() {
	return getUniqueIdentifier();
    }

    public String getTitle() {
	return title;
    }

    public void setAuthor(String author) {
	this.author = author;
    }

    public void setTitle(String title) {
	this.title = title;
    }

    @Override
    public String toInspectString() {
	return String.format("'%s' by %s (ISBN: %s)", getTitle(), getAuthor(), getIsbn());
    }

    @Override
    public String toString() {
	return new ToStringBuilder(this, ToStringStyle.NO_FIELD_NAMES_STYLE).append(author)
		.append(title).append(uniqueIdentifier).toString();
    }
}
