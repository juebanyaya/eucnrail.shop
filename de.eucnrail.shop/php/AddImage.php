<?php
    
	$key  = $_GET['key'];
	$name  = $_GET['name'];
	$id = $_GET['id'];
	echo $key.'____'.$name.'_____'.$id
	$image_path = '/var/www/html/images/'.$_GET['path'].'.JPG';
    $urlImage = 'https://eucnrail.de/tao/api/images/products/'.$id;
    //Here you set the path to the image you need to upload

    $image_mime = 'image/jpg';
    $args['image'] = new CurlFile($image_path, $image_mime);
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_HEADER, 1);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
    curl_setopt($ch, CURLINFO_HEADER_OUT, 1);
    curl_setopt($ch, CURLOPT_URL, $urlImage);
    curl_setopt($ch, CURLOPT_POST, 1);
    curl_setopt($ch, CURLOPT_USERPWD, $key.':');
    curl_setopt($ch, CURLOPT_POSTFIELDS, $args);
    $result = curl_exec($ch);
    $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    curl_close($ch);

    if (200 == $httpCode) {
        echo 'Product image was successfully created.';
    }
	else
	{
		echo $httpCode;
	}