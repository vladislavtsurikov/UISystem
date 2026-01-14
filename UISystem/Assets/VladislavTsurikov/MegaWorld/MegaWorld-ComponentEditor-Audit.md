# MegaWorld ComponentEditor Audit (CustomInspector migration)

## Область проверки
Ниже перечислены **все** `ComponentEditor` в MegaWorld (по `ElementEditor`/`ReorderableListComponentEditor`), которые сейчас существуют в репозитории:

- `AlignEditor` (`Editor/Common/Settings/TransformElementSystem/Elements/AlignEditor.cs`)
- `TilesEditor` (`Editor/Common/Settings/ScatterSystem/Elements/TilesEditor.cs`)
- `RandomGridEditor` (`Editor/Common/Settings/ScatterSystem/Elements/RandomGridEditor.cs`)
- `RotationEditor` (`Editor/BrushModifyTool/GUI/ModifyTransformComponents/Components/RotationEditor.cs`)
- `TexturesFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/TexturesFilterEditor.cs`)
- `NoiseFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/NoiseFilterEditor.cs`)
- `MaskOperationsFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/MaskOperationsFilterEditor.cs`)
- `ImageFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/ImageFilterEditor.cs`)
- `ConcavityFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/ConcavityFilterEditor.cs`)
- `HeightNoiseFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/HeightNoiseFilterEditor.cs`)
- `HeightFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/HeightFilterEditor.cs`)
- `AspectFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/AspectFilterEditor.cs`)
- `SlopeFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/SlopeFilterEditor.cs`)

`MaskFilterEditor` — базовый абстрактный класс для фильтров, UI там нет, поэтому он не классифицируется как «простой/сложный».

---

## Простые ComponentEditor → можно заменить на CustomInspector

| Editor (файл) | Target компонент (файл) | Почему простой | Как заменить CustomInspector |
| --- | --- | --- | --- |
| `RandomizeScaleEditor` (`Editor/BrushModifyTool/GUI/ModifyTransformComponents/Components/RandomizeScaleEditor.cs`) | `RandomizeScale` (`Editor/BrushModifyTool/ModifyTransformComponents/Elements/RandomizeScale.cs`) | Логика UI сводится к min/max слайдеру и переключателю «Uniform», что уже поддерживается `MinMaxSliderAttribute` и `UniformToggleFieldName` в CustomInspector. | Заменено: перенесено на `MinScale` `MinMaxSlider(..., UniformToggleFieldName = nameof(UniformScale))`, `MaxScale` скрыт, редактор удален. |
| `ScaleClampEditor` (`Editor/BrushModifyTool/GUI/ModifyTransformComponents/Components/ScaleClampEditor.cs`) | `ScaleClamp` (`Editor/BrushModifyTool/ModifyTransformComponents/Elements/ScaleClamp.cs`) | UI — это стандартный min/max слайдер с подписями и парами числовых полей. Это напрямую совпадает с `MinMaxSliderAttribute`. | Заменено: `MinScale` использует `MinMaxSlider(..., nameof(MaxScale))`, `MaxScale` скрыт, редактор удален. |

---

## Сложные ComponentEditor → нужны ComponentEditor (или новые drawers)

| Editor (файл) | Почему сложный / что мешает CustomInspector |
| --- | --- |
| `AlignEditor` (`Editor/Common/Settings/TransformElementSystem/Elements/AlignEditor.cs`) | В одном и том же поле нужны **две** разных модели отображения (min/max слайдер *или* одиночный слайдер) в зависимости от `MinMaxRange`. Для этого требуется условная смена drawer’а или разделение данных, чего сейчас нет. |
| `TilesEditor` (`Editor/Common/Settings/ScatterSystem/Elements/TilesEditor.cs`) | Используется `Vector2.Max` для принудительного минимума на `Vector2`. CustomInspector пока не умеет валидацию/кламп для `Vector2` атрибутами. |
| `RandomGridEditor` (`Editor/Common/Settings/ScatterSystem/Elements/RandomGridEditor.cs`) | UI пересчитывает `GridStep` на основе `UniformGrid` и скрывает/открывает группы полей в зависимости от `RandomisationType`. Это смесь вычисляемых полей и условной логики, которую нельзя выразить только атрибутами. |
| `RotationEditor` (`Editor/BrushModifyTool/GUI/ModifyTransformComponents/Components/RotationEditor.cs`) | Отрисовка идёт **по осям** внутри `Vector3` (разные поля для X/Y/Z в зависимости от bool), то есть нужен кастомный drawer «по компонентам вектора». |
| `TexturesFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/TexturesFilterEditor.cs`) | Использует `Terrain.activeTerrain`, scroll view с иконками, контекстное меню, обработку событий и предпросмотр — это невозможно повторить атрибутами. |
| `NoiseFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/NoiseFilterEditor.cs`) | Делегирует UI в `NoiseSettingsGUI`, включая превью/панели с динамической высотой. |
| `MaskOperationsFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/MaskOperationsFilterEditor.cs`) | Switch-логика по enum, разные типы UI (min/max слайдеры, одиночные слайдеры) и динамическое доп. имя в списке. |
| `ImageFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/ImageFilterEditor.cs`) | Валидация формата текстуры, предупреждения `HelpBox`, разные режимы UI по состоянию. |
| `ConcavityFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/ConcavityFilterEditor.cs`) | Вычисляемая ширина лейблов + `CurveField` с преобразованием кривой в текстуру (side effect). |
| `HeightNoiseFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/HeightNoiseFilterEditor.cs`) | Комбинирует настройки высоты + `NoiseSettingsGUI` с предпросмотрами и подпанелями. |
| `HeightFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/HeightFilterEditor.cs`) | Зависимость от `index` (скрытие BlendMode), а также вложенная условная логика falloff (min/max/one value). |
| `AspectFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/AspectFilterEditor.cs`) | `CurveField` + обновление текстуры при изменении кривой, плюс `index`-условие для BlendMode. |
| `SlopeFilterEditor` (`Editor/Common/Settings/FilterSettings/MaskFilterSystem/Elements/SlopeFilterEditor.cs`) | Проверки наличия Terrain/Draw Instanced, min/max слайдер с кастомными подписями и условные falloff-поля. |

---

## Вывод
- **Заменены** `RandomizeScaleEditor` и `ScaleClampEditor` — их UI покрыт возможностями CustomInspector.
- Остальные — **сложные**, т.к. требуют контекста списка (`index`), предпросмотра/валидации, динамических панелей или специфической отрисовки (например, покомпонентно для `Vector3`). Для них нужен либо `ComponentEditor`, либо расширение CustomInspector новыми drawer’ами/атрибутами.
