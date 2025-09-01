import { NavigationItem } from './navigation.type';

export const defaultNavigation: NavigationItem[] = [
  {
    id: 'dashboard',
    title: 'menu.dashboard',
    type: 'basic',
    icon: 'dashboard',
    link: '/dashboard',
  },
  {
    id: 'projects',
    title: 'menu.projects',
    type: 'basic',
    icon: 'folder',
    link: '/projects',
  },
  {
    id: 'tasks',
    title: 'menu.tasks',
    type: 'basic',
    icon: 'task_alt',
    link: '/tasks',
  },
  {
    id: 'pomodoros',
    title: 'menu.pomodoros',
    type: 'basic',
    icon: 'timer',
    link: '/pomodoros',
  },
  // {
  //   id: 'pomodoros.group',
  //   title: 'menu.pomodoros',
  //   icon: 'mdi:xxxx',
  //   type: 'collapsable',
  //   children: [
  //     {
  //       id: 'pomodoros',
  //       title: 'menu.pomodoros',
  //       type: 'basic',
  //       icon: 'mdi:xxxx',
  //       link: '/pomodoros',
  //     },
  //   ],
  // },
];
