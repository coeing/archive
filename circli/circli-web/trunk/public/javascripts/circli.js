function setupArticle(articleElement)
{
	// Find id.
	var articleIdElement = articleElement.find(".articleId");
	if (articleIdElement == null)
	{
		alert("Found article without id element.");
		return;
	}
	
	var articleFunctionsElement = articleElement.find(".articleFunctions");
	
	var articleId = articleIdElement.text();

	articleElement.mouseenter( function() 
			{ 
				$.ajax({url: "/uiController/functioncircle",
					    data: { "articleId": articleId },
					    dataType: "html",
						success: function(data) 
							{ 
								articleFunctionsElement.html(data);
							}
						});
				
			});
	
	articleElement.mouseleave( function() 
			{ 
				articleFunctionsElement.html("");				
			});
}