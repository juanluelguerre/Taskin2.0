# Taskin 2.0

# How to add new components

Sample for an 'component-name' page component, always in a **standalone** mode:

```

```

## ng-matero

- https://github.com/ng-matero/extensions

```
ng add @angular/material
npm install @ng-matero/extensions --save
```

## How to

- Development server :Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.
- Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum --standalone --skip-tests --inline-style --change-detection OnPush --view-encapsulation None`. Sample: `ng g c modules/organizations/pages/component-name --standalone --skip-tests --inline-style --change-detection OnPush --view-encapsulation None`
- Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.
- Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).
- Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.
- To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
