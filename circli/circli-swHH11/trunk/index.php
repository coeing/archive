<?php
include 'header.php';
include 'Database.php';
?>
<style>
<!--
div#top {
	margin-top: 200px;
	margin-bottom: 115px;
}
-->
</style>
	<script type="text/javascript">
			$(function(){
				var bookArray = [<?php foreach ($bookDB as $book) {
					echo "\"".$book->author . " - " . $book->title."\",";
				}?>];

				//Autocomplete
				$(".search").autocomplete({
					source: bookArray,
					position: { 
						my: "center top",
					    at: "center bottom",
						offset: "36 -39",
					    collision: "fit"
					},
					open: function(event, ui) { $('.ui-autocomplete').css('width','459px'); }
				});
				
				$(function () {
					$(window).load(function () {
						$(".search").attr('placeholder', 'Boy, do I want to give this book away');
					});
				});

				$('#give').click(function() {
					$(".search").val("");
					$(".search").attr('placeholder', 'Boy, do I want to give this book away');
					$('#get').removeClass("current");
					$(this).addClass("current");
					$('#nav').css('left', '-63px');
                    var actionField = $('#action');
                    actionField.val("give");
				});
				
				$('#get').click(function() {
					$(".search").val("");
					$(".search").attr('placeholder', 'I want to have this book');
					$('#give').removeClass("current");
					$(this).addClass("current");
					$('#nav').css('left', '57px');
					var actionField = $('#action');
                    actionField.val("get");
				});

				$('#submitform').keypress(function(event) {
					  $('#submitform').submit();
					});
			});
		</script>	
		
		<div id="floater"></div>		
		<div id="content">
			<form name="submitform" action="result.php" method="post">
			<input type="hidden" id="action" name="action" value="give"/>
			<input class="search" />
			<div id="nav">
			<img alt="foo" id="navIndicator" src="images/searchbox_PFEIL.png">
			</div>
				<ul id="navlist">
					<li id="active"><a href="#give" id="give" class="current">Give</a></li>
					<li><a href="#get" id="get">Get</a></li>
				</ul>
			</form>
		</div>
		
		<div id="footer">
			<a href="#help">Do you need an explanation?</a>
		</div>
<?php
include 'footer.php';
?>