export interface NavigationItem {
  id?: string;
  title?: string;
  subtitle?: string;
  type: 'aside' | 'basic' | 'collapsable' | 'divider' | 'group' | 'spacer';
  hidden?: (item: NavigationItem) => boolean;
  active?: boolean;
  tooltip?: string;
  link?: string;
  externalLink?: boolean;
  icon?: string;
  children?: NavigationItem[];
  meta?: any;
}
