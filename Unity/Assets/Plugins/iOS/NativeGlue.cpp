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

	void _setSize(int _width, int _height);
	
	void _setTextureFlag(bool isDraw);
	
	
	//char *hoge_(const char* str);
	
	//void init_(const char* str);
	
	int getX_();
	int getY_();
	int getZ_();
	//void update_();
	
	
	//void* getCamera();
	//void releaseCamera(void* camera);
	//void getCameraTexture(void* camera, unsigned char* data, int width, int height);
}

// とりあえずグローバルに置いておけば万事解決

const double DETECT_SCALE = 4.0;

int facePos[] = {0,0,0};
//std::string cascade_file;

bool isDebug = false;

cv::VideoCapture *cap = 0;
cv::CascadeClassifier *cascade =0;

bool isDrawTex = false;
int  width = 0;
int  height = 0;
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
	//isDrawTex = isDraw;
	//texW = width;
	//texH = height;
	
	if(texImg){delete texImg; texImg=0;}
	if(isDraw)texImg = new cv::Mat(height, width, CV_8UC1);
}

void _setSize(int _width, int _height){
	width  = _width;
	height = _height;
}

void _setTextureFlag(bool isDraw){
	if(texImg){
		delete texImg;
		texImg=0;
	}
	
	if(isDraw){
		texImg = new cv::Mat(height, width, CV_8UC1);
	}
}


void _update()
{
	cv::Mat img;
	cv::Mat gray;
	
	// カメラ画の取得
	*cap >> img;
	
	
	//scaleの値を用いて元画像を縮小
	double scale = 4.0;
	
	
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
		facePos[0] = cv::saturate_cast<int>((f.x + f.width*0.5)*DETECT_SCALE);
		facePos[1] = cv::saturate_cast<int>((f.y + f.height*0.5)*DETECT_SCALE);
	}
	
	
	// 結果の描画
	if( !faces.empty() ) {
		auto f = faces.front();      //  最初の要素
		cv::Point center(getX_(), getY_());
		int radius = cv::saturate_cast<int>((f.width + f.height)*0.25*DETECT_SCALE);
		
		cv::circle( gray, center, radius, cv::Scalar(255,255,255), 3, 8, 0 );
	}
	
	
	
	facePos[2] = rand()%10;
	
	
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

//===============================

/*

char* hoge_(const char* str){
	
	//std::string = str;
	
	
	
	
	char* res = (char*)malloc(strlen(str)+1);
	strcpy(res, str);
	return res;
}


void init_(const char* str)
{
	cascade_file = str;
	
	cascade = new cv::CascadeClassifier();
	
	if(! cascade->load(cascade_file)){
		return;
	}
}


void update_(){
	
	cv::VideoCapture cap(0);
	
	
	cv::CascadeClassifier cascade;
	if(!cascade.load(cascade_file)){
		return;
	}
	
	
	//scaleの値を用いて元画像を縮小
	double scale = 1.0;
	
	
	cv::Mat img;
	cv::Mat gray;
	
	cap >> img;
	
	// グレースケール画像に変換
	cv::cvtColor(img, gray, CV_BGR2GRAY);
	cv::Mat smallImg(cv::saturate_cast<int>(img.rows/scale), cv::saturate_cast<int>(img.cols/scale), CV_8UC1);
	
	// 処理時間短縮のために画像を縮小
	cv::resize(gray, smallImg, smallImg.size(), 0, 0, cv::INTER_LINEAR);
	cv::equalizeHist( smallImg, smallImg);
	
	
	std::vector<cv::Rect> faces;
	
	cascade.detectMultiScale(gray, faces, 1.1, 2,CV_HAAR_SCALE_IMAGE, cv::Size(30, 30));
	
	
	// 結果の描画
	std::vector<cv::Rect>::const_iterator r = faces.begin();
	for(; r != faces.end(); ++r)
	{
		facePos[0] = cv::saturate_cast<int>((r->x + r->width*0.5)*scale);
		facePos[1] = cv::saturate_cast<int>((r->y + r->height*0.5)*scale);
		facePos[2] = rand()%10;
	}
	
	
	
	//facePos[0] = rand()%10;
	//facePos[1] = rand()%10;
	//facePos[2] = rand()%10;
}









// 画像表示する系

void* getCamera()
{
	cap = new cv::VideoCapture(1);
	
	return static_cast<void*>(cap);
}

void releaseCamera(void* camera)
{
	//auto vc = static_cast<cv::VideoCapture*>(camera);
	//delete vc;
	delete cap;
	delete cascade;
}


void getCameraTexture(void* camera, unsigned char* data, int width, int height)
{
	//auto vc = static_cast<cv::VideoCapture*>(camera);
	
	//cv::CascadeClassifier *cascade = new cv::CascadeClassifier();
	//if(! cascade->load(cascade_file)){
	//		return;
	//	}
	
	
	cv::Mat img;
	cv::Mat gray;
	
	
	// カメラ画の取得
	//*vc >> img;
	*cap >> img;
	
	
	//scaleの値を用いて元画像を縮小
	double scale = 4.0;
	
	
	// グレースケール画像に変換
	cv::cvtColor(img, gray, CV_BGR2GRAY);
	cv::Mat smallImg(cv::saturate_cast<int>(img.rows/scale), cv::saturate_cast<int>(img.cols/scale), CV_8UC1);
	
	// 処理時間短縮のために画像を縮小
	cv::resize(gray, smallImg, smallImg.size(), 0, 0, cv::INTER_LINEAR);
	cv::equalizeHist( smallImg, smallImg);	// 暗すぎたりする画像にはヒストグラムの均一化ってのをするといいらしい
	
	std::vector<cv::Rect> faces;
	
	cascade->detectMultiScale(smallImg, faces, 1.1, 2,CV_HAAR_SCALE_IMAGE, cv::Size(30, 30));
	
	// 座標代入
	if( !faces.empty() ) {
		auto f = faces.front();      //  最初の要素
		facePos[0] = cv::saturate_cast<int>((f.x + f.width*0.5)*scale);
		facePos[1] = cv::saturate_cast<int>((f.y + f.height*0.5)*scale);
	}
	
	
	// 結果の描画
	if( !faces.empty() ) {
		auto f = faces.front();      //  最初の要素
		cv::Point center(getX_(), getY_());
		int radius = cv::saturate_cast<int>((f.width + f.height)*0.25*scale);
		
		cv::circle( gray, center, radius, cv::Scalar(255,255,255), 3, 8, 0 );
	}
	
	
	
	facePos[2] = rand()%10;
	
	
	// リサイズ
	cv::Mat resized_img(height, width, gray.type());
	cv::resize(gray, resized_img, resized_img.size(), cv::INTER_CUBIC);
	
	// RGB --> ARGB 変換
	cv::Mat argb_img;
	cv::cvtColor(resized_img, argb_img, CV_GRAY2BGRA);
	
	
	std::memcpy(data, argb_img.data, argb_img.total() * argb_img.elemSize());
}
*/