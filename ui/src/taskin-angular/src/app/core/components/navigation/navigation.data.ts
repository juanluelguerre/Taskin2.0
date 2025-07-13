import { NavigationItem } from './navigation.type';

export const defaultNavigation: NavigationItem[] = [
  {
    id: 'dashboard',
    title: 'menu.dashboard',
    type: 'basic',
    icon: 'mdi:view-dashboard-variant-outline',
    link: '/dashboard',
  },
  {
    id: 'projects',
    title: 'menu.projects',
    type: 'basic',
    icon: 'mdi:view-dashboard-variant-outline',
    link: '/projects',
  },
  {
    id: 'tasks',
    title: 'menu.tasks',
    type: 'basic',
    icon: 'mdi:view-dashboard-variant-outline',
    link: '/tasks',
  },
  {
    id: 'pomodoros',
    title: 'menu.pomodoros',
    type: 'basic',
    icon: 'mdi:view-dashboard-variant-outline',
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
