# DrawAndPainting_VR

<img width="597" alt="image" src="https://user-images.githubusercontent.com/59603575/102004661-a3d90780-3cc7-11eb-894c-fd19d713e5a1.png">

## 프로젝트 개요  
- VR게임상에서 그림을 그리고 그 그림을 딥러닝 모델로 판별하고 다시 VR환경에 관련 오브젝트를 띄운다. 이후, 스프레이, 색연필로 오브젝트로 색칠하여 전시하는 프로젝트

**특징**
- python의 pytorch Framwork를 사용하여 낙서 데이터들을 학습
- Unity의 ML-Agent을 python과 통신에 이용
- 그림그리기 및 오브젝트 색칠

---

## UML

<img width="1175" alt="image" src="https://user-images.githubusercontent.com/59603575/102004689-daaf1d80-3cc7-11eb-8e5c-ad96fd1a79b5.png">

---

## 개발내용

### 딥러닝 (데이터셋, 모델 선언, 훈련)

[구글 Quick Draw Dataset](https://github.com/googlecreativelab/quickdraw-dataset)

[모델 repo](https://github.com/Kwonkunkun/DrawAndPainting_Pytorch)

<img width="693" alt="image" src="https://user-images.githubusercontent.com/59603575/102004731-1d70f580-3cc8-11eb-806e-d42a565506ba.png">

- Google에서 제공하는 quickdraw dataset 
- .npy형태를 받아 선언한 resnet 모델로 트레이닝
- 파이썬 코드는 추가로 커밋예정

### VR환경구성

<img width="794" alt="image" src="https://user-images.githubusercontent.com/59603575/102004767-717bda00-3cc8-11eb-8fe2-b85f40fa1cee.png">

1. 그림을 테블릿에 아래에 존재하는 연필을 통해 그릴 수있음.
2. 출력 시 3D프린터 모델에서 인식된 모델이 나옴.
3. 색연필, 스프레이, HVLP를 통해 오브젝트를 자유롭게 색칠.
4. 그린 그림을 전시.

---

## 사용기술, 맡은 역할
**사용기술**
- Unity
- SteamVR
- Vive Pro 

**맡은 역할**
- Python 딥러닝 모델, 학습 구현
- VR 환경 구현
Player 구현
