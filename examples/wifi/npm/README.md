## Работа с pnpm workspaces
В папке npm настроен прототип монорепозитория pnpm.
Установка pnpm:
```
npm install -g pnpm
```

Управление пакетами из папки package может осуществляться из корневой папки рабочей области (папка npm) или из собственной папки каждого пакета.
Используйте флаг --filter для выборочного запуска команды. Пордробнее [тут](https://pnpm.io/ru/filtering#--filter-package_name). Пример:
```
pnpm --filter "inline-js" build
```

### Доступные команды
Установка зависимостей:
```
pnpm install
```
Production сборка - выполняет команду pnpm run build во всех вложенных пакетах, если она там есть.
```
pnpm run build
```
Watch режим - выполняет команду pnpm run watch во всех вложенных пакетах, если она там есть.
```
pnpm run watch
```

### Публикация пакетов
Для управления версиями публикуемых пакетов нет встроеного решения, в докмуентации ссылаются на [changesets](https://github.com/changesets/changesets) и [Rush](https://rushjs.io/)

ссылка на документацию - https://pnpm.io/ru/workspaces