# 📍 AR Navigation (Unity + Kakao Mobility API)

Unity **AR Foundation**을 기반으로 실외 보행자용 AR 내비게이션을 구현한 프로젝트입니다.  
GPS와 나침반 데이터를 결합하여 사용자 현재 위치를 추적하고, 카카오모빌리티 API로 받아온 경로를 실제 물리 공간에 AR 화살표로 투영합니다.

> **핵심 설계**: 시각적 피로도를 줄이기 위해 전체 경로 중 **사용자 전방 5m**만 AR로 표시하며, 경로 이탈 시 실시간 재탐색 및 역방향(U-Turn) 감지 기능을 제공합니다.

---

## 🧭 주요 기능

- ✅ **GPS 기반 위치 추적**: Android 환경에서 정밀한 사용자 위치 실시간 동기화.
- ✅ **Kakao Mobility API 연동**: 도보 길찾기 API를 통한 최적 경로 데이터 수신.
- ✅ **전방 5m AR 가이드**: 전체 경로를 유지하되, AR 시각화는 사용자 주변 5m로 제한하여 최적화.
- ✅ **경로 이탈 및 역방향 감지**: 사용자가 경로를 벗어나거나 반대로 걸을 때(U-Turn) 즉각 감지.
- ✅ **지능형 재탐색(Reroute)**: 이탈 판단 시 자동으로 경로 재요청 및 업데이트.
- ✅ **GPS 오차 보정**: 신호 불안정으로 인한 '가짜 이탈'을 방지하는 필터링 로직 적용.

---

## 🛠 개발 환경

| 항목 | 내용 |
| :--- | :--- |
| **Unity Version** | 2022.3 LTS 이상 권장 |
| **Platform** | Android (ARCore) |
| **AR Framework** | AR Foundation |
| **API** | Kakao Mobility Directions (Walk) |
| **Language** | C# |

---

## 📂 프로젝트 폴더 구조

```text
Assets/
 ├─ Scenes/
 │   └─ ARNavigationScene.unity
 ├─ Scripts/
 │   ├─ Core/
 │   │   └─ ARNavigationManager.cs       # 내비게이션 전체 상태 제어
 │   ├─ Location/
 │   │   └─ LocationServiceManager.cs   # GPS/나침반 데이터 관리
 │   ├─ Navigation/
 │   │   ├─ RouteManager.cs             # API 호출 및 전체 경로 데이터 관리
 │   │   ├─ RoutePoint.cs               # 좌표 데이터 모델
 │   │   ├─ RerouteManager.cs           # 이탈 판단 및 재요청 로직
 │   │   └─ DirectionAnalyzer.cs        # U-Turn 및 방향 분석
 │   ├─ Rendering/
 │   │   └─ ARPathRenderer.cs           # AR 화살표 생성 및 거리별 활성화
 │   ├─ UI/
 │   │   └─ NavigationUIManager.cs      # 안내 문구 및 거리 정보 표시
 │   └─ Utils/
 │       └─ GeoUtils.cs                 # 위경도 <-> Unity 좌표 변환 유틸
 ├─ Prefabs/
 │   └─ Arrow.prefab                    # AR 경로 가이드 화살표
 └─ Materials/                          # 에셋 재질 및 쉐이더
```
## 🚀 실행 방법 (Android)

### 1️⃣ Unity 프로젝트 열기
* **Unity Hub** 실행 → **Open** → 프로젝트 폴더 선택
* Unity 버전은 **2022.3 LTS** 이상을 권장합니다.

### 2️⃣ Android 권한 및 빌드 설정
* **File > Build Settings**에서 플랫폼을 **Android**로 변경
* **Player Settings > Android** 탭 설정:
    * **Location**: `Fine Location` 및 `Coarse Location` 권한 확인
    * **Internet Access**: `Require` (API 통신용)
    * **Minimum API Level**: Android 8.0 (Oreo) 이상 권장

### 3️⃣ AR Scene 구성 (Hierarchy)
씬 내 오브젝트 구성을 아래와 같이 배치합니다. `GPSOrigin`은 항상 좌표 `(0,0,0)`을 유지해야 합니다.



* **XR Origin (AR)**: AR 카메라 및 세션 관리
* **GPSOrigin (Empty Object)**: 월드 기준점 (Position: 0, 0, 0)
    * └ `LocationServiceManager`: GPS 데이터 처리
* **NavigationManager**: 전체 흐름 제어
    * └ `ARNavigationManager`: 상태 제어
* **RouteManager**: Kakao API 호출 및 경로 데이터 관리
* **ARPathRenderer**: 화살표 생성 및 렌더링
* **NavigationUI**: 거리 및 상태 안내 UI

### 4️⃣ Inspector 상세 설정
애플리케이션의 동작 특성을 인스펙터 창에서 조정합니다.

* **📍 RouteManager**
    * `End Lat Lng`: 목적지 위경도 입력 (X: 위도, Y: 경도)
    * *주의: 현재 위치 기준 50~300m 거리의 실외 좌표 권장*
* **📍 ARPathRenderer**
    * `Arrow Prefab`: 가이드용 화살표 프리팹 할당
    * `Spacing`: 화살표 간격 (1.0 권장)
* **📍 ARNavigationManager**
    * `Visible Distance`: **5** (전방 표시 거리)
    * `Arrive Threshold`: **3** (도착 판정 거리)
    * `Off Route Threshold`: **6** (경로 이탈 판정 거리)

### 5️⃣ Build & Run
* Android 기기를 USB 또는 무선으로 연결합니다.
* **Build And Run** 실행 후 기기에서 다음 권한을 허용합니다:
    1. 위치 권한 (정확한 위치 사용)
    2. 카메라 권한 (AR 시각화용)
* 반드시 **실외**에서 테스트해야 GPS 오차를 최소화할 수 있습니다.

---

## ⚠️ 주의사항
* **환경 제약**: 실내나 고층 빌드 숲에서는 GPS 오차로 인해 경로가 튈 수 있습니다.
* **시각화 로직**: 목적지가 멀더라도 AR 경로는 항상 현재 위치 기준 **전방 5m**만 표시됩니다.
* **배터리 및 발열**: AR과 GPS를 동시에 사용하여 배터리 소모가 빠르므로 주의가 필요합니다.

---

## 📌 향후 확장 예정
- [ ] 지도 UI에서 목적지 직접 선택 기능 추가
- [ ] 실시간 재탐색 경로 시각화 애니메이션
- [ ] iOS (ARKit) 환경 대응
- [ ] 건물 입구 및 좁은 길 정밀 안내 모드
- [ ] AR Occlusion(가림 처리) 적용으로 현실감 증대

---

## 👤 개발자 메모
본 프로젝트는 **Flutter에서 Unity로 전환**하는 과정에서 얻은 위경도 좌표계 변환 및 GPS 최적화 노하우를 바탕으로 설계되었습니다.<br>
단순한 위치 표시를 넘어, 실제 보행 환경에서 신뢰할 수 있는 사용자 경험(UX)을 제공하는 것을 목표로 합니다.
