package models;

import play.*;
import play.db.jpa.*;

import javax.persistence.*;

import org.apache.commons.lang.builder.ReflectionToStringBuilder;
import org.apache.commons.lang.builder.ToStringBuilder;
import org.apache.commons.lang.builder.ToStringStyle;

import java.util.*;

@MappedSuperclass
public class BaseModel extends Model {
    @Temporal(TemporalType.TIMESTAMP)
    @Column(nullable = false)
    private Date createdAt;

    @Temporal(TemporalType.TIMESTAMP)
    @Column(nullable = false)
    private Date updatedAt;

    @PrePersist
    protected void onCreate() {
	updatedAt = createdAt = new Date();
    }

    @PreUpdate
    protected void onUpdate() {
	updatedAt = new Date();
    }

    public Date getCreatedAt() {
	return createdAt;
    }

    public Date getUpdatedAt() {
	return updatedAt;
    }

    public String toInspectString() {
	return toString();
    }
}
