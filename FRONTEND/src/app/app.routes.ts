import { Routes } from '@angular/router';
import { LoginComponent } from './components/authentication/login/login';
import { RegisterComponent } from './components/authentication/register/register';
import { AdminDashboardComponent } from './components/admin/dashboard/dashboard';
import { ManagerDashboardComponent } from './components/manager/dashboard/manager-dashboard';

import { authGuard } from './guards/auth-guard';
import { LearningPaths } from './components/admin/learning-paths/learning-paths';
import { SendNotifications } from './components/admin/send-notifications/send-notifications';
import { Settings } from './components/admin/settings/settings';
import { ManageCourses } from './components/admin/manage-courses/manage-courses';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'forgot-password', loadComponent: () => import('./components/authentication/forgot-password/forgot-password').then(m => m.ForgotPassword) },
  { path: 'reset-password', loadComponent: () => import('./components/authentication/reset-password/reset-password').then(m => m.ResetPassword) },
  { path: 'admin/dashboard', component: AdminDashboardComponent, canActivate: [authGuard] },
  { path: 'admin/users', loadComponent: () => import('./components/admin/manage-users/manage-users').then(m => m.ManageUsersComponent), canActivate: [authGuard] },
  { path: 'admin/departments', loadComponent: () => import('./components/admin/manage-departments/manage-departments').then(m => m.ManageDepartments), canActivate: [authGuard] },
  { path: 'admin/learning-paths', loadComponent: () => import('./components/admin/learning-paths/learning-paths').then(m => m.LearningPaths), canActivate: [authGuard] },
  { path: 'admin/send-notifications', loadComponent: () => import('./components/admin/send-notifications/send-notifications').then(m => m.SendNotifications), canActivate: [authGuard] },
  { path: 'admin/settings', loadComponent: () => import('./components/admin/settings/settings').then(m => m.Settings), canActivate: [authGuard] },
  { path: 'admin/manage-courses', loadComponent: () => import('./components/admin/manage-courses/manage-courses').then(m => m.ManageCourses), canActivate: [authGuard] },
  { path: 'admin/quizzes', loadComponent: () => import('./components/admin/manage-quizzes/manage-quizzes').then(m => m.ManageQuizzesComponent), canActivate: [authGuard] },
  { path: 'admin/profile', loadComponent: () => import('./components/admin/profile/profile').then(m => m.ProfileComponent), canActivate: [authGuard] },
  { path: 'admin-dashboard', redirectTo: '/admin/dashboard', pathMatch: 'full' },
  { 
    path: 'manager', 
    loadComponent: () => import('./components/manager/layout/manager-layout/manager-layout').then(m => m.ManagerLayout),
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', component: ManagerDashboardComponent },
      { path: 'team', loadComponent: () => import('./components/manager/team-management/team-management').then(m => m.TeamManagementComponent) },
      { path: 'learning-paths', loadComponent: () => import('./components/manager/learning-paths/learning-paths').then(m => m.ManagerLearningPathsComponent) },
      { path: 'reports', loadComponent: () => import('./components/manager/reports/reports').then(m => m.ManagerReportsComponent) },
      { path: 'weekly-hours', loadComponent: () => import('./components/manager/weekly-hours/weekly-hours').then(m => m.WeeklyHoursComponent) },
      { path: 'approvals', loadComponent: () => import('./manager/approvals/approvals-list/approvals-list').then(m => m.ApprovalsList) },
      { path: 'approvals/history', loadComponent: () => import('./manager/approvals/approvals-history/approvals-history').then(m => m.ApprovalsHistory) },
      { path: 'profile', loadComponent: () => import('./components/manager/profile/profile').then(m => m.ManagerProfileComponent) },
    ]
  },
  { path: 'manager-dashboard', redirectTo: '/manager/dashboard', pathMatch: 'full' },
  { path: 'manager/approvals', redirectTo: '/manager/approvals', pathMatch: 'full' },
  {
    path: 'employee',
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', loadComponent: () => import('./components/employee/dashboard/employee-dashboard').then(m => m.EmployeeDashboardComponent) },
      { path: 'courses', loadComponent: () => import('./components/employee/courses/courses').then(m => m.CoursesComponent) },
      { path: 'course-viewer/:id', loadComponent: () => import('./components/employee/course-viewer/course-viewer').then(m => m.CourseViewer) },
      { path: 'learning-paths', loadComponent: () => import('./components/employee/learning-paths/learning-paths').then(m => m.EmployeeLearningPathsComponent) },
      { path: 'modules/:id', loadComponent: () => import('./components/employee/modules/modules').then(m => m.ModulesComponent) },
      { path: 'quiz/:id', loadComponent: () => import('./components/employee/quiz/quiz').then(m => m.QuizComponent) },
      { path: 'notes/:id', loadComponent: () => import('./components/employee/notes/notes').then(m => m.NotesComponent) },
      { path: 'progress', loadComponent: () => import('./components/employee/progress/progress').then(m => m.ProgressComponent) },
      { path: 'weekly-hours', loadComponent: () => import('./components/employee/weekly-hours/weekly-hours').then(m => m.WeeklyHoursComponent) },
      { path: 'certificates', loadComponent: () => import('./components/employee/certificates/certificates').then(m => m.CertificatesComponent) },
      { path: 'flashcards/:id', loadComponent: () => import('./components/employee/flashcards/flashcards').then(m => m.FlashcardsComponent) },
      { path: 'course-request', loadComponent: () => import('./components/employee/course-request/course-request').then(m => m.CourseRequest) },
      { path: 'profile', loadComponent: () => import('./components/employee/profile/profile').then(m => m.ProfileComponent) }
    ]
  },
  { path: 'employee-dashboard', redirectTo: '/employee/dashboard', pathMatch: 'full' },
  { path: 'landing', loadComponent: () => import('./components/landing/landing').then(m => m.Landing) },
  { path: '', redirectTo: '/landing', pathMatch: 'full' }
];
