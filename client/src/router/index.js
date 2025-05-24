import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import Login from '@/views/Login.vue'
import Register from '../views/Register.vue'
import Projects from '../views/Projects.vue'
import CreateProject from '../views/CreateProject.vue'
import EditProject from '../views/EditProject.vue'
import ProjectDetails from '../views/ProjectDetails.vue'
import CreateTask from '../views/CreateTask.vue'
import EditTask from '../views/EditTask.vue'
import MyTasks from '../views/MyTasks.vue'
import TaskLogs from '../views/MyLogs.vue'
import TaskDetails from '../views/TaskDetails.vue'
import { useAuthStore } from '@/stores/authStore'; // adjust the path based on your project
import CreateWorkspace from '../views/CreateWorkspace.vue'
import Workspace from '../views/Workspace.vue'
import JoinWorkspace from '../views/JoinWorkspace.vue'


const routes = [
    {path: '/', name: 'home', component: HomeView },
  { path: '/login', component: Login },
  { path: '/register', component: Register },
  { path: '/workspaces/create', name: 'createWorkspace', component: CreateWorkspace}, 
  { path: '/workspaces/join', name: 'joinWorkspace', component: JoinWorkspace}, 
  { path: '/workspaces/:workspaceId', name: 'workspace', component: Workspace},
  { path: '/workspaces/:workspaceId/projects/create', name: 'createProject', component: CreateProject},
  { path: '/workspaces/:workspaceId/projects/:projectId', name: 'project', component: ProjectDetails },
  { path: '/workspaces/:workspaceId/projects/:projectId/edit', name: 'editProject', component: EditProject },
  { path: '/workspaces/:workspaceId/projects/:projectId/tasks/create', name: 'createTask', component: CreateTask },
  { path: '/workspaces/:workspaceId/projects/:projectId/tasks/:taskId', name: 'task',  component: TaskDetails },
  { path: '/workspaces/:workspaceId/projects/:projectId/tasks/:taskId/edit', name: 'editTask', component: EditTask },
  { path: '/workspaces/:workspaceId/my-tasks', name: 'myTasks', component: MyTasks},
  {path: '/workspaces/:workspaceId/my-tasks/:taskId/logs', name: 'myLogs', component: TaskLogs },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})


router.beforeEach((to, from, next) => {
  const auth = useAuthStore();

  const publicPages = ['/login', '/register'];
  const authRequired = !publicPages.includes(to.path);
  const isLoggedIn = auth.isAuthenticated;

  if (authRequired && !isLoggedIn) {
    return next({ 
      path: '/login',
      query: { redirect: to.fullPath }
    });
  }

  next();
});


export default router
