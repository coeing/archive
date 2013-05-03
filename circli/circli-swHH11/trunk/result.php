<?php
include 'header.php';
?>
<style>
body {
	overflow-x: auto;
	overflow-y: auto;
}
</style>
<?php

require_once 'Book.class.php';
require_once 'Database.php';

//$text = $_REQUEST["search"];
$action = isset($_REQUEST["action"]) ? $_REQUEST["action"] : "get";

$myBook = $bookDB[0];
$otherBooks = array();
foreach ($bookDB as $book)
{
	if ($book != $myBook)
	{
		array_push($otherBooks, $book);
	}
}

?>
<style>
body {
    overflow-x: auto;
    overflow-y: auto;
}
</style>
<div class="box" id="swap_box">  

<?php if ($action == "give") { ?>
 <!-- <h2>You have this book: <?=$myBook->FullTitle()?>.</h2> -->
 <h2 class="header">Give it to the Circle</h2>
 <p class="green">Congratulations, you can get one of these books:</p> 
<?php } else { ?>
 <!-- <h2>You want this book: <?=$myBook->FullTitle()?>.</h2> -->
 <h2 class="header">Get it from the Circle</h2>
 <p class="green">You can get it if you have one of these books:</p>
<?php } ?>
 <script>
 $(document).ready(function(){
		
		//$("#book-carousel").CloudCarousel( { 
		//	reflHeight: 56,
		//	reflGap:2,
		//	buttonLeft: $('#but1'),
		//	buttonRight: $('#but2'),
		//	yRadius:40,
		//	xPos: 285,
		//	yPos: 120,
		//	speed:0.15,
		//	mouseWheel:true
		//});

		 <?php 
		$idx = 0;
		foreach ($otherBooks as $book)
		{
		?>
		$('#book-carousel2 a#book_<?=$idx?>').tooltip({
			track: true,
			delay: 0,
			showURL: false,
			showBody: " - ",
			bodyHandler: function() { 
		        return "<?=$book->FullTitle()?>"; 
		    },
			fade: 0
		});	
		$('#book-carousel2 a#book_<?=$idx?>').click(function(event) {
			$('#swap_box').html('<h3 class="header" id="congrats">Congrats!</h3>'+
                    '<p class="green">You can close a circle. Click \'swap now\' to make it happen.</p>'+
                    '<div id="circle_box">'     +               
                    '    <table>'+
                    '        <tr>'+
                    '       <td class="neighbour_left"><p>by: <a href="profile.php">Julia</a></p></td>'+
                    '       <td class="you"><p>by: <a href="profile.php">You</a></p></td>'+
                    '       <td class="neighbour_right"><p>by: <a href="profile.php">Jan</a></p></td>'+
                    '       </tr>'+
                    '   </table>'+
                    '   </div>'+
                    '   <a id="submit" href="confirmed2.php"></a>');
		});
		 <?php 
		 	++$idx;
		 }
		 ?>	

		jQuery('#book-carousel2').jcarousel({
	    	wrap: 'circular'
	    });
	});
 </script>
 
 <!-- 
 <div id="book-carousel" style="width:570px; height:384px;background: url(/images/carousel/bg.jpg);overflow:scroll;">
	 
 <ul> 
 <?php 
foreach ($otherBooks as $book)
{
?>
	<a href="cycle.php" rel='lightbox' title="<?=$book->author?> - <?=$book->title?>"><img class="cloudcarousel" src="<?=$book->image?>" width="128" height="164"/></a>
<?php 
}
?>        
       
          
<div id="but1" class="carouselLeft" style="position:absolute;top:20px;right:64px;"></div>

 			<div id="but2" class="carouselRight" style="position:absolute;top:20px;right:20px;"></div>      
 	</div>
 -->
 
<ul id="book-carousel2" class="jcarousel-skin-tango">
 <?php 
$idx = 0;
foreach ($otherBooks as $book)
{
?>
	<li><a id="book_<?=$idx?>" href="#" rel='lightbox' title="<?=$book->author?> - <?=$book->title?>"><img src="<?=$book->image?>" width="105" height="160" alt="" /></a></li>
<?php 
  ++$idx;
}
?>  
</ul>

</div> 

<h1 class="or">- OR -</h1>

<div class="box" id="buy_box">  
	 <h2 class="header">Order this on Amazon</h2>
	 <p class="green">By this you help us surviving. We love surviving.</p>
 
    <a id="submit" href="http://www.amazon.co.uk/Hitchhikers-Guide-Galaxy-Douglas-Adams/dp/1417642599/"></a>
</div> 

<?php
include 'footer.php';
?>

