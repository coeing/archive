package models.circle;

import java.util.Date;

import javax.persistence.Entity;
import javax.persistence.ManyToOne;
import javax.persistence.OneToOne;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import models.BaseModel;
import models.user.item.OwnedItem;
import models.user.item.WantedItem;

import org.apache.commons.lang.builder.ReflectionToStringBuilder;

@Entity
public class SwapCircleStep extends BaseModel {
    @ManyToOne(optional = false)
    private SwapCircle circle;

    @OneToOne(optional = false)
    private OwnedItem ownedItem;

    @OneToOne(optional = false)
    private WantedItem wantedItem;

    @Temporal(TemporalType.TIMESTAMP)
    private Date ownedItemSentAt;

    @Temporal(TemporalType.TIMESTAMP)
    private Date ownedItemReceivedAt;

    @Temporal(TemporalType.TIMESTAMP)
    private Date wantedItemReceivedAt;

    @Temporal(TemporalType.TIMESTAMP)
    private Date wantedItemSentAt;

    public SwapCircleStep(OwnedItem ownedItem, WantedItem wantedItem) {
	this.ownedItem = ownedItem;
	this.wantedItem = wantedItem;
    }

    public SwapCircle getCircle() {
	return circle;
    }

    public void setCircle(SwapCircle circle) {
	this.circle = circle;
    }

    public WantedItem getWantedItem() {
	return wantedItem;
    }

    public void setWantedItem(WantedItem wantedItem) {
	this.wantedItem = wantedItem;
    }

    public OwnedItem getOwnedItem() {
	return ownedItem;
    }

    public void setOwnedItem(OwnedItem ownedItem) {
	this.ownedItem = ownedItem;
    }

    @Override
    public String toString() {
	return toInspectString();
    }

    @Override
    public String toInspectString() {
	return String.format("%s gives %s to %s", getOwnedItem().getUser().toInspectString(),
		getOwnedItem().getArticle().toInspectString(), getWantedItem().getUser()
			.toInspectString());
    }

    public Date getOwnedItemSent() {
	return ownedItemSentAt;
    }

    public void setOwnedItemSent(Date ownedItemSent) {
	this.ownedItemSentAt = ownedItemSent;
    }

    public Date getWantedItemReceived() {
	return wantedItemReceivedAt;
    }

    public void setWantedItemReceived(Date wantedItemReceived) {
	this.wantedItemReceivedAt = wantedItemReceived;
    }

    public Date getOwnedItemReceivedAt() {
	return ownedItemReceivedAt;
    }

    public void setOwnedItemReceivedAt(Date ownedItemReceivedAt) {
	this.ownedItemReceivedAt = ownedItemReceivedAt;
    }

    public Date getWantedItemSentAt() {
	return wantedItemSentAt;
    }

    public void setWantedItemSentAt(Date wantedItemSentAt) {
	this.wantedItemSentAt = wantedItemSentAt;
    }
}