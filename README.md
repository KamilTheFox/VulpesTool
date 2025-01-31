# VulpesTool - Мощный инструментарий для Unity Editor 🚀

Рад представить вам набор инструментов, который сделает работу в редакторе Unity более удобной и эффективной!

# ✨ Что внутри:

📍 Умные кнопки

Создавайте кнопки для методов одним атрибутом
Гибкая настройка расположения кнопок для полей
Полная кастомизация внешнего вида

# 🎨 Визуальная организация

Группировка полей в стильные боксы
Интуитивная система вложенности
Поддержка заголовков и цветового оформления


# 🔗 InterfaceReference

Элегантное решение для сериализации интерфейсов
Никаких костылей - чистая и понятная архитектура
Полная поддержка Inspector-окна


# 📝 Продвинутый Enum-селектор

Умный поиск по значениям
Группировка элементов для удобной навигации
Полностью кастомизируемое отображение


# 🎯 Система стилизации

Простая покраска любых элементов
Гибкие настройки цветов
Поддержка градиентов и прозрачности



# Сделайте ваш редактор Unity удобнее с VulpesTool! 🦊✨

Примечание:
Некоторые аттрибуты работают только если компонент унаследован от VulpesMonoBehaviour или VulpesScriptableObject

# Инструкция по применению

## Начало работы

Для того, что бы инструмент работал в вашем MonoBehaviour и ScreptableObject

нужно использовать using VulpesTool

и унаследоваться от класса VulpesMonoBehaviour или ScreptableObject.

## Список атрибутов:

###- - BeginBox - Начало визуального разделения инспектора

### -- EndBox - конец визуального разделения инспектора (Если не обозначить границы, до бокс растянется до конца полей)

### -- Button - Отобразить кнопку в инспекторе для метода

### -- ButtonField - Отобразить кнопку для поля (Обязательно нужно указать имя исполняемого метода для кнопки)

### -- EnumCategory - Выделить перечислители в категорию (Категория указывается до следующего указания EnumCategory)

### -- EnumSearchWindow - справа от поля создает кнопку для более удобного выбора Enum

- поддерживает атрибут Flags

### -- CreateGUI - используется для создания кастомного редактора из MonoBehaviour. 

- используется для выведения динамической информации, дебага и дополнительного стилизованного интерфейса. 

### -- FindAtScene - для поиска объекта на сцене. Запуск поиска происходит из Tools/VulpesTool/UpdateReferencesScene.

### -- ViewOnly - Для блокирования изменения поля (Продолжает отображаться в инспекторе, без возможности изменить поле из инспектора)

### -- SelectImplementation - создает кнопку для выбора реализации абстрактного объекта 

- используется в связке с SerializeReference;

### -- InterfaceReference<> дает оболочку для интерфейса на MonoBehaviour. Если MonoBehaviour не реализует интерфейс, поле даст ошибку и не изменится.


## Примеры использования

```C#
    [BeginBox("Player Settings", color: ColorsGUI.Red)]
    [ButtonField(nameof(HealthRemove), position: ButtonPosition.Left,buttonText: "-100")]
    [ButtonField(nameof(HealthAdd),position: ButtonPosition.Right, buttonText: "+100")]
    [ViewOnly]
    [SerializeField]
    private float health;
    [SerializeField]
    private float speed;
    [SerializeField]
    private string playerName;
    [EndBox]
```

```C#
    [SelectImplementation]
    [SerializeReference]
    private List<IInterface> Interface;
```

```C#
    [EnumSearchWindow]
    public GameActions actions1;

    [EnumSearchWindow]
    public Test TestFlagEnum;


    [Flags]
    public enum Test
    {
        flag1 = 1 << 1,
        flag2 = 1 << 2,
        flag3 = 1 << 3,
        flag4 = 1 << 4,
        flag5 = 1 << 5,
        flag6 = 1 << 6,
        flag7 = 1 << 7,
        flag8 = 1 << 8,
    }

    public enum GameActions
    {
        None = 0,

        // Combat Actions
        [EnumCategory("Combat")]
        Attack = 100,
        Defend = 101,
        CastSpell = 102,
        UseItem = 103,
        Dodge = 104,
        Parry = 105,
        SpecialAttack = 106,

        // Movement Actions
        [EnumCategory("Movement")]
        Walk = 200,
        Run = 201,
        Jump = 202,
        Crouch = 203,
        Climb = 204,
        Swim = 205,
        Dash = 206,
        Slide = 207,

        // Interaction Actions
        [EnumCategory("Interaction")]
        Talk = 300,
        Trade = 301,
        PickUp = 302,
        Open = 303,
        Close = 304,
        Examine = 305,
        UseObject = 306,
        Craft = 307
    }
```

```C#
    public InterfaceReference<ITest> testObjects;
```

```C#
    [Button(order: 1,color: ColorsGUI.Dark)]
    private void CteateTest1()
    {
       // действие
    }

    [Button(order: 1, color: ColorsGUI.Blue)]
    private void CteateTest2()
    {
        // действие
    }

    [Button(order: 1, color: ColorsGUI.Green)]
    private void CteateTest3()
    {
        // действие
    }
    [Button(order: 0, color: ColorsGUI.Red)]
    private void CteateClear()
    {
        // действие
    }
```

```C#
    [CreateGUI(title: "Info Item", color: ColorsGUI.InfoBlue)]
    private void CustomEditorGUI()
    {
        GUIStyle gUI = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        GUILayout.Label("Stationary: " + this.Stationary + "; Type Entity: " + this.typeEntity.ToString(), gUI);
        if (this is AnimalEngine engine)
        {
            GUILayout.Label($"TypeAnimal: {engine.TypeAnimal}", gUI);
            GUILayout.Label($"AI: {engine.NameAI} / Behavior: {engine.Behavior}", gUI);
            GUILayout.Label($"All Animal: {AnimalEngine.AnimalList.Count}", gUI);
        }
        if (this is ItemEngine Item)
        {
            GUILayout.Label($"TypeItem: {Item.itemType}", gUI);
            if (Item.isController)
                GUILayout.Label($"IsPlayerController", gUI);
            GUILayout.Label($"All Item: {ItemEngine.GetItems.Length}", gUI);
        }
        if (this is PlantEngine plant)
        {
            GUILayout.Label($"TypePlant: {plant.typePlant}", gUI);
            GUILayout.Label($"All Plant: {PlantEngine.GetPlants.Length}", gUI);
        }
    }
```

```C#
    [VulpesTool.SelectImplementation]
    [SerializeReference]
    private List<IAdditionalAnimate> additionalAnimates = new List<IAdditionalAnimate>();
```


## Будущее: 

Я очень хочу

в скором времени добавить поддержку нестандартных структур данных, по типу Dictionary, HashSet, Queue, Stack и тд

Переработать InterfaceReference<> в атрибут для сериализации, как с SerializeReference