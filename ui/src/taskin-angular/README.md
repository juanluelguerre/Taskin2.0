@@ -1,27 +1,27 @@

# Taskin 2.0

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 18.0.3.

# How to add new components

## Development server

Sample for an 'component-name' page component, always in a **standalone** mode:

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

```

## Code scaffolding
```

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

```
ng add @angular/material
```

## Running unit tests

## How to

- Where find flags: https://github.com/Yummygum/flagpack-core
- Where find icons: https://fonts.google.com/icons?icon.set=Material+Symbols

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.

- Development server :Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.
- Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum --standalone --skip-tests --inline-style --change-detection OnPush --view-encapsulation None`. Sample: `ng g c modules/organizations/pages/component-name --standalone --skip-tests --inline-style --change-detection OnPush --view-encapsulation None`
- Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.
- Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).
- Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.
- To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
