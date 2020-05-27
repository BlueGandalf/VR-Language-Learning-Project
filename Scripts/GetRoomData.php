<?php
    // Configuration
    $hostname = 'localhost';
    $username = 'cops';
    $password = 'Ha2YpTChsJ';
    $database = 'cops';
	
	$link = mysqli_connect($hostname , $username , $password ,$database);
	if($link === false) {
		die("ERROR: Could not connect. " . mysqli_connect_error());
	}
	$RoomID = $_REQUEST['RoomNumber'];
	
    //get characters
	$sql = "SELECT * FROM TblCharacter WHERE RoomID = $RoomID";
	if( $result = mysqli_query($link, $sql)) {
		while($row = mysqli_fetch_assoc($result)) {
			$characterID = $row["CharacterID"];
		}
	}else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
    }
    //get conversations
    $data = "";
	$sql = "SELECT * FROM TblConversation WHERE CharacterID = $characterID";
	if( $result = mysqli_query($link,$sql)) {
		while($row = mysqli_fetch_assoc($result)) {
			//write that into a string
        $data .= $row['ConversationID'] . "," . $row['WordLevel'] . "," . $row['TextID'] . "," . $row['CharacterID'] . "," . $row['ItemID'] . "~";
		}
	} else {
		echo "ERROR: Could not able to execute " . $sql. mysqli_error($link);
    }
    //get items
	$data = rtrim($data, '~');
	$data .= ";";
	$sql = "SELECT * FROM TblItem WHERE RoomID = $RoomID";
	if ( $result = mysqli_query($link , $sql)) {
		while($row = mysqli_fetch_assoc($result)) {
			//write that into a string
            $data .= $row['ItemID'] . "," . $row["ItemName"] . "," . $row["WordID"] . "," . $row["RoomID"] . "~";
		}
    }
    //get words
	$data = rtrim($data, '~');
	$data .= ";";
	$sql = "SELECT * FROM TblWord WHERE RoomID = $RoomID";
	if ( $result = mysqli_query($link , $sql)) {
		while($row = mysqli_fetch_assoc($result)) {
			//write that into a string
			$data .= $row['WordID'] . "," . $row["L1Text"] . "," . $row["L2Text"] . "," . $row["L2Audio"] . "," . $row["RoomID"] . "~";
		}
	}
	//get answer
	$data = rtrim($data, '~');
	$data .= ";";
	$sql = "SELECT AnswerID, TblConversation.ConversationID, TblAnswer.TextID, CorrectAnswer FROM TblAnswer JOIN TblConversation ON TblConversation.ConversationID = TblAnswer.ConversationID WHERE TblConversation.CharacterID = $characterID ";
	if ( $result = mysqli_query($link , $sql)) {
		while($row = mysqli_fetch_assoc($result)) {
			//write that into a string
            $data .= $row['AnswerID'] . "," . $row["ConversationID"] . "," . $row["TextID"] . "," . $row["CorrectAnswer"] . "~";
		}
	}
	//echo that string back
	$data = rtrim($data, '~');
	echo $data;
	mysqli_close($link);
?>