INCLUDE globals.ink

-> main


=== main ===
조작 컨트롤을 선택해주세요.
+[키보드]
    -> keyboard
    
    
+[패드]
    -> gamepad
    
    
=== keyboard ===
~ pauseDialogue()
WASD로 이동할 수 있습니다.

~ pauseDialogue()
Space로 회피할 수 있습니다.

~ pauseDialogue()
마우스 좌클릭으로 공격할 수 있습니다.

~ pauseDialogue()
마우스 우클릭으로 강공격이 가능합니다. 기를 모으면 더욱 강해집니다.

~ pauseDialogue()
키보드 R로 체력을 회복할 수 있습니다.

적 위에 표시되는 붉은 색 게이지는 체력, 주황 색 게이지는 경직 게이지 입니다.
~ pauseDialogue()
경직 게이지가 0이 되면 적은 경직됩니다.

~ pauseDialogue()
경직 성능이 높은 공격일 수록 적의 경직 시간이 길어집니다.

스킬을 사용하기 위해선 마나가 필요합니다.
~ pauseDialogue()
일반 공격이나 강공격으로 적을 공격하면 마나를 회복할 수 있습니다.

~ pauseDialogue()
shift + <sprite=42>, \\nshift + <sprite=43>, \\nshift + <sprite=44>, \\nshift + <sprite=45>으로 각각 할당된 스킬을 사용할 수 있습니다.

~ pauseDialogue()
마우스 휠 클릭으로 적을 락온할 수 있습니다.

~ pauseDialogue()
마우스 휠 스크롤로 락온 변경이 가능합니다.


더 자세한 내용은 메뉴를 열어 도움말을 읽어주세요.
->END



=== gamepad ===
~ pauseDialogue()
왼쪽 스틱<sprite=28>로 이동할 수 있습니다.

~ pauseDialogue()
<sprite=21>버튼으로 회피할 수 있습니다.

~ pauseDialogue()
<sprite=23>버튼으로 공격할 수 있습니다.

~ pauseDialogue()
<sprite=22>버튼으로 강공격이 가능합니다. 기를 모으면 더욱 강해집니다.

~ pauseDialogue()
<sprite=20>버튼으로 체력을 회복할 수 있습니다.


적 위에 표시되는 붉은 색 게이지는 체력, 주황 색 게이지는 경직 게이지 입니다.
~ pauseDialogue()
경직 게이지가 0이 되면 적은 경직됩니다.

~ pauseDialogue()
경직 성능이 높은 공격일 수록 적의 경직 시간이 길어집니다.

스킬을 사용하기 위해선 마나가 필요합니다.
~ pauseDialogue()
일반 공격이나 강공격으로 적을 공격하면 마나를 회복할 수 있습니다.

~ pauseDialogue()
<sprite=25> + <sprite=23>버튼, \\n<sprite=25> + <sprite=22>버튼, \\n<sprite=25> + <sprite=21>버튼, \\n<sprite=25> + <sprite=20>버튼으로 각각 할당된 스킬을 사용할 수 있습니다.

~ pauseDialogue()
오른쪽 스틱 <sprite=29>버튼으로 적을 락온할 수 있습니다.

~ pauseDialogue()
<sprite=24>, <sprite=26>로 락온 변경이 가능합니다.


더 자세한 내용은 메뉴를 열어 도움말을 읽어주세요.
-> END
