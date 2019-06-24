<?php
		wh_log("do the request...");
		$response = array();		
		if($_POST['action_name']== 'add_image')
		{
			try
			{
				$url=$_POST['img_url'];						
				$name = $_POST['image_name']; 
				$url =  $_POST['image_location'] . $name;
				$new = 'images/'.$name;
				upload_image($url, $new);
				$response[0] = array(
					'id' => '1',
					'msg'=> "uploaded the image '" . $url . "' successfully'",
					'code'=> '1'
				);	
			}
			catch(Exception $e)
			{
				$response[0] = array(
					'id' => '1',
					'msg'=> $e->getMessage(),
					'code'=> '0'
				);
			}
			try
			{
				$targetUrl = $_POST['target_url'];
				$apiKey = $_POST['api_key'];
				$imageMine = $_POST['image_mine']; 
				add_productimage($targetUrl, $apiKey, $new, $imageMine);
				$response[1] = array(
					'id' => '2',
					'msg'=> "created image for the product '" . $targetUrl. "' successfully.",
					'code'=> '1'
				);					
			}
			catch(Exception $e)
			{
				$response[1] = array(
					'id' => '2',
					'msg'=> $e->getMessage(),
					'code'=> '0'
				);				
			}
			echo json_encode($response);
		}

	function upload_image($url, $new)
	{
		wh_log("image uploading...");
		wh_log("url = " . $url);
		$data = file_get_contents($url);
		$rawdata = file_get_contents($url, false, $context);
		if ($rawdata === false) 
		{
			throw new Exception("Unable to update data at $url");
		}
		file_put_contents($new, $data);
		wh_log("uploaded image successfully");	
	}
	
	function add_productimage($targetUrl, $apiKey, $imagePath, $imageMine)
	{
			wh_log("adding image to product...");
			wh_log("targetUrl = " . $targetUrl);
			wh_log("apiKey = " . $apiKey);
			wh_log("imagePath = " . $imagePath);
			wh_log("imageMine = " . $imageMine);
    		$args['image'] = new CurlFile($imagePath, $imageMine);
			$ch = curl_init();
    		curl_setopt($ch, CURLOPT_HEADER, 1);
    		curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
    		curl_setopt($ch, CURLINFO_HEADER_OUT, 1);
			curl_setopt($ch, CURLOPT_URL, $targetUrl);
    		curl_setopt($ch, CURLOPT_POST, 1);
    		curl_setopt($ch, CURLOPT_USERPWD, $apiKey.':');
    		curl_setopt($ch, CURLOPT_POSTFIELDS, $args);
    		$result = curl_exec($ch);
			$errors = curl_error($ch);
    		$httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    		curl_close($ch);
		if (200 == $httpCode) 
		{
        		wh_log("created product image successfully.");	
		}
		else
		{
			wh_log(var_dump($errors));			
			throw New Exception(var_dump($errors));
		}

	}
	function remove_image()
	{

	}
	function wh_log($log_msg) 
	{
		$log_filename = "images";
		$log_file_data = $log_filename.'/log_' . date('d-M-Y') . '.log';
		file_put_contents($log_file_data,  date('Y-m-d H:i:sO ').$log_msg. "\n", FILE_APPEND);
	}

?>
