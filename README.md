Клас <b>Unit</b> відповідає за юніта. У юніта є евристики (це деякі параметри цього юніта, які обчислюються на кожному кроці, наприклад, відстань до супротивників), біхейвіори-поведінки (які основуються на евристиках, наприклад, поведінка штурму), екшини (дії-вчинки, наприклад, удар мечем).
Екшини є MonoBehavior-класами, а евристики та біхейвіори - ні.

<b>GridPosition</b> - це структура, яка репрезентує ігрову комірку, використовується для пошуку шляху (Pathfinding) та в принципі ходів гравця.

Класи-нащадки від <b>BaseHeuristics</b> - це евристики, займаються видачею якогось int-значення методом GetValue для використання біхейвіорами. Наприкоад, евристика SuperiorityHeuristic (перевага) повертає різницю між кількістю ворожих та союзних юнітів.

Класи-нащадки від <b>BaseBehavior</b> - це біе=хейвіори, займаються вибором екшинів в залежності від значення евристик. Також мають метод GetValue, який повертає важливість використання саме цієї поведінки. Наприклад, якщо HitClosestEnemyBehavior використовуючи евристику ClosestEnemyDistanceHeuristic бачить, що поруч є ворог, він повертає більше значення своєї важливості, а якщо ворогів поруч немає, то повертає 0.

Класи-нащадки від <b>BaseAction</b> - це конкретні дії бота чи гравця. Класи реалізують метод GetEnemyAIAction, який повертає EnemyAIAction, в якому у змінній value зберігається пріорітетність цього екшина відповідно певній позиції на полі бою. Так, збираючи ці значення, екшини типу MoveOffenciveAction повертають пріорітетність напрямку руху, потім біхейвіори обирають як використовувати ці дані для прийняття рішення на яку конкретно комірку рухатись відповідно цих значень.
З цікавого можна глянути до реалізації MoveOffenciveAction, та методу ShortestPathToEnemy - він використовує клас Pathfinding для пошуку шляхів до ворогів і обирає найкоротший із них.

Для реалізації AI основним класом виступає <b>EnemyAI</b>. Він наявний на сцені у двох екземплярах, відповідно один оперує юнітам гравця, інший юнітами суперника.
На старті EnemyAI підписується на дві основгні події: TurnSystem.Instance.OnTurnChanged - коли відбувається зміна ходу, та UnitManager.Instance.OnAllEnemiesDead - коли всі юніти однієї з команд загинули.

Основний метод цього класу - <b>TryTakeEnemyAIAction</b>. В ньому відбувається прийняття рішення щодо руху юніта. Для кожного юніта із команди застосовуємо метод TryTakeEnemyAIAction. 

Метод DefaultActionChoosing реалізовує вибір екшинина по старій системі, без використання евристик та біхейвіорів.
Метод BehaviorActionChoosing реалізовує вибір екшина по новій системі з використанням евристик та біхейвіорів.
В методі <b>BehaviorActionChoosing</b> ми використовуємо вибір біхейвіора за допомогою метода ChooseBehaviorUsingSoftmax (Softmax-метод). 
Також в класі є ChooseBehaviorUsingEpsilonGreedy - реалізація епсілон-жадібної вибірки.

Навчання ботів реалізоване у записі всіх ходів всіх юнітів у окремі файли. При старті ігрової сесії створюється папка із її датою та часом, в цій папці в окремих файлах ходи юнітів в форматі JSON. 

Хід юніта серіалізується за допомогою класу <b>TrainingData</b>: номер ходу, позиція юніта, значення всіх евристик, значення всіх біхейвіорів, обраний біхейвіор за цих умов та нагорода. Нагорода - це різниця у ХП між початком та кінцем ходу юніта. Всі ходи юніта за ігрову сесію зберігаються у екземплярі класу <b>TrainingDataWrapper</b>.

Зберігання у файл і вигрузка з файлу реалізовані у класі <b>Database</b>.

Прогнозування ботів реалізоване у класі <b>Prediction</b> в методі <b>GetAveragePlayerPositionAtMove</b>, який повертає усереднену позикію всіх юнітів команди гравця на конкретний хід, таким чином прогнозуючи позицію.

Адаптивність ботів реалізоване у класі <b>Prediction</b> в методі <b>GetAverageTotalPlayerHealth</b>, який повертає середнє сумарне значення ХП юнітів команди гравця. Таким чином, визначаючи чи зазвичай на певний конкретний хід гравця він брав участь у бойових діях і порівнявши із поточним сумарним значення ХП юнітів, можна визначити чи гравець в цей раз грає агресивніше чи пасивніше, ніж у середньому за попередні ігри.
