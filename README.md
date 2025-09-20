# Unity Android 프로젝트

## 📱 프로젝트 개요
Unity를 사용하여 개발하는 안드로이드 애플리케이션 프로젝트입니다.

## 🛠️ 개발 환경
- **엔진**: Unity 2022.3 LTS (권장)
- **플랫폼**: Android
- **언어**: C#
- **최소 Android 버전**: API 21 (Android 5.0)

## 📁 프로젝트 구조
```
UnityAndroidProject/
├── Assets/
│   ├── Scripts/        # C# 스크립트 파일들
│   ├── Prefabs/        # 프리팹 파일들
│   ├── Materials/      # 머티리얼 파일들
│   ├── Textures/       # 텍스처 및 이미지
│   ├── Audio/          # 오디오 파일들
│   ├── Scenes/         # 게임 씬 파일들
│   ├── Animations/     # 애니메이션 파일들
│   └── Fonts/          # 폰트 파일들
├── ProjectSettings/    # Unity 프로젝트 설정
├── Packages/          # Package Manager 패키지들
├── .gitignore         # Git 제외 파일 설정
└── README.md          # 프로젝트 문서
```

## 🚀 시작하기

### 필수 요구사항
1. Unity Hub 설치
2. Unity 2022.3 LTS 에디터 설치
3. Android SDK 및 NDK 설치
4. JDK 설치

### 프로젝트 설정
1. Unity Hub에서 "Add project from disk" 선택
2. 이 폴더를 선택하여 프로젝트 열기
3. File → Build Settings에서 Android 플랫폼 선택
4. Player Settings에서 Android 관련 설정 구성

### 빌드 설정
- **Package Name**: com.yourcompany.yourapp
- **Version Code**: 1
- **Minimum API Level**: Android API level 21
- **Target API Level**: Android API level 33 (권장)

## 📋 개발 가이드라인

### 코딩 컨벤션
- C# 네이밍 컨벤션 준수
- 스크립트는 Assets/Scripts 폴더에 정리
- MonoBehaviour 상속 클래스는 GameObject에 연결하여 사용

### Git 사용법
```bash
# 프로젝트 클론
git clone <repository-url>

# 변경사항 커밋
git add .
git commit -m "커밋 메시지"
git push origin main
```

## 🔧 자주 사용하는 Unity 패키지
- TextMeshPro
- Unity Analytics
- Unity Ads
- Unity In-App Purchasing

## 📝 할 일 목록
- [ ] 기본 게임 씬 생성
- [ ] UI 시스템 구현
- [ ] 게임 로직 개발
- [ ] Android 권한 설정
- [ ] 테스트 빌드
- [ ] 플레이스토어 배포 준비

## 📞 연락처
프로젝트에 대한 문의사항이 있으시면 언제든지 연락주세요.

---
*이 README는 프로젝트 진행에 따라 계속 업데이트됩니다.*
