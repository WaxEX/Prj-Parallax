#include<string.h>
#include<stdlib.h>
#include <opencv2/opencv.hpp>
#include <cstdio>


extern "C"{
	
	bool _awakeCamera(const char* cascade_file, int camera_index = 0);
	void _releaseCamera();

	void _update();
	
	void _setTextureInfo(bool isDraw, int width=0, int height=0);
	void _getTexture(unsigned char* data);
	
	int getX_();
	int getY_();
	int getZ_();
}

// とりあえずグローバルに置いておけば万事解決

const double DETECT_SCALE = 4.0;

int facePos[] = {0,0,0};

cv::VideoCapture *cap = 0;
cv::CascadeClassifier *cascade =0;

cv::Mat *texImg = 0;

bool _awakeCamera(const char* cascade_file, int camera_index)
{
	cap = new cv::VideoCapture(camera_index);
	if(!cap){
		return false;
	}
	
	cascade = new cv::CascadeClassifier();
	if(! cascade->load(cascade_file)){
		return false;
	}
	
	return true;
}

void _releaseCamera()
{
	if(cap)     delete cap;
	if(cascade) delete cascade;
	
	if(texImg)  delete texImg;
}

void _setTextureInfo(bool isDraw, int width, int height){
	if(texImg){delete texImg; texImg=0;}
	if(isDraw)texImg = new cv::Mat(height, width, CV_8UC1);
}

void _update()
{
	cv::Mat img;
	cv::Mat gray;
	
	// カメラ画の取得
	*cap >> img;
	
	
	//scaleの値を用いて元画像を縮小
	
	// グレースケール画像に変換
	cv::cvtColor(img, gray, CV_BGR2GRAY);
	cv::Mat smallImg(cv::saturate_cast<int>(img.rows/DETECT_SCALE), cv::saturate_cast<int>(img.cols/DETECT_SCALE), CV_8UC1);
	
	// 処理時間短縮のために画像を縮小
	cv::resize(gray, smallImg, smallImg.size(), 0, 0, cv::INTER_LINEAR);
	cv::equalizeHist( smallImg, smallImg);	// 暗すぎたりする画像にはヒストグラムの均一化ってのをするといいらしい
	
	// 検出実行
	std::vector<cv::Rect> faces;
	cascade->detectMultiScale(smallImg, faces, 1.1, 2,CV_HAAR_SCALE_IMAGE, cv::Size(30, 30));
	
	// 座標代入
	if( !faces.empty() ) {
		auto f = faces.front();      //  最初の要素
		
		int x = cv::saturate_cast<int>((f.x + f.width*0.5)*DETECT_SCALE);
		int y = cv::saturate_cast<int>((f.y + f.height*0.5)*DETECT_SCALE);
		
		// 中心からの差分
		facePos[0] = x - cv::saturate_cast<int>(img.cols/2);
		facePos[1] = y - cv::saturate_cast<int>(img.rows/2);
		
		// ===ごめんなさい。本当はこんなところで座標の比率を調整するのはダメなんだけど、時間が無いの…
		facePos[0] /= 10;
		facePos[1] /= 10;
		
		
		// 結果の描画
		cv::Point center(x, y);
		int radius = cv::saturate_cast<int>((f.width + f.height)*0.25*DETECT_SCALE);
		cv::circle( gray, center, radius, cv::Scalar(255,255,255), 3, 8, 0 );
		
	}
	
	if(texImg){
		cv::Mat resized_img(texImg->rows, texImg->cols, gray.type());
		cv::resize(gray, resized_img, resized_img.size(), cv::INTER_CUBIC);
		
		// RGB --> ARGB 変換
		cv::cvtColor(resized_img, *texImg, CV_GRAY2BGRA);
	
	}
}


void _getTexture(unsigned char* data)
{
	std::memcpy(data, texImg->data, texImg->total() * texImg->elemSize());
}

int getX_(){
	return facePos[0];
}

int getY_(){
	return facePos[1];
}

int getZ_(){
	return facePos[2];
}