package controllers;

import models.article.Article;
import models.article.Book;
import play.Logger;

public class UIController extends BaseController {

    public static void functionCircle(Long articleId) {
	Article article = new Book("Test", "Test", "Test");
	article.id = articleId;
	functionCircle(article);
    }

    private static void functionCircle(Article article) {
	Logger.debug("Showing function circle for article %s", article.id);
	render(article);
    }

    public static void like(Long articleId) {
	// OwnedItem item = OwnedItem.findById(itemId);
	Article article = new Book("Test", "Test", "Test");
	article.id = articleId;
	like(article);
    }

    private static void like(Article article) {
	Logger.debug("Liking article %s", article.id);
    }

    public static void own(Long articleId) {
	// OwnedItem item = OwnedItem.findById(itemId);
	Article article = new Book("Test", "Test", "Test");
	article.id = articleId;
	own(article);
    }

    private static void own(Article article) {
	Logger.debug("Owning article %s", article.id);
    }

    public static void ignore(Long articleId) {
	// OwnedItem item = OwnedItem.findById(itemId);
	Article article = new Book("Test", "Test", "Test");
	article.id = articleId;
	ignore(article);
    }

    private static void ignore(Article article) {
	Logger.debug("Wanting article %s", article.id);
    }

    public static void info(Long articleId) {
	// OwnedItem item = OwnedItem.findById(itemId);
	Article article = new Book("Test", "Test", "Test");
	article.id = articleId;
	info(article);
    }

    private static void info(Article article) {
	Logger.debug("Wanting article %s", article.id);
    }

}
