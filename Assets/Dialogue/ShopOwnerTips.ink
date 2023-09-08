INCLUDE globals.ink
VAR whichTip = 0

{shopOwnerEvent:
- "first": -> first
- "clearEasy": -> clearEasy
- "clearNormal": -> clearNormal
- "clearHard": -> clearHard
- else: -> tip
}

=== first ===
~ shopOwnerEvent = ""
안녕 만나서 반가워.
던전으로 들어가기 전에 스텟과 스킬을 한번씩 확인해 보는게 좋을거야.
스텟, 스킬, 장비, 상점 등등 필요한 게 있으면 나를 찾아와.
-> END

=== tip ===
~ whichTip = RANDOM(0, 4)
{whichTip:
- 0: 스텟을 올리면 기본 능력치 이외에도 스킬이 가지고 있는 스텟 보정치에 따라서 데미지가 증가하지.
- 1: 경직 성능이 낮은 공격은 적의 경직 게이지가 0이 되어도 경직을 발생하지 않을 수도 있어.
- 2: 마법은 불, 얼음, 번개, 무속성으로 나뉘어지고 무속성을 제외한 마법은 속성치가 쌓이면 속성에 따라서 각각 다른 효과가 발동해.
- 3: 경직 중인 적을 공격하면 더 큰 데미지를 줄 수 있지.
- 4: 이곳에서 오른쪽 아래로 내려가면 작은 훈련장이 하나 있어.
}
-> END

=== clearEasy ===
~ shopOwnerEvent = ""
쉬움 난이도를 클리어했군.
스킬 장착 슬롯과 스텟 포인트가 늘어났으니까 확인해봐.
액티브 스킬 변경은 키보드는 Q로, 패드는 LT/RT로 전환이 가능해.
새로운 스킬과 아이템도 추가 됐으니까 한번 둘러보면 좋을거야.
-> END

=== clearNormal ===
~ shopOwnerEvent = ""
보통 난이도 클리어 축하한다.
이번에도 스킬 슬롯과 스텟 포인트가 늘어났으니 확인해봐.
새로운 스킬과 아이템도 확인하고 말이야.
-> END

=== clearHard ===
모든 난이도 클리어 축하해.
게임을 플레이 해줘서 고맙다.
-> END


