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


const routes = [
    {path: '/', name: 'home', component: HomeView },
  { path: '/login', component: Login },
  { path: '/register', component: Register },
  { path: '/admin/projects', component: Projects },
  { path: '/admin/projects/create', component: CreateProject },
  { path: '/admin/projects/:id', component: ProjectDetails },
  { path: '/admin/projects/:id/edit', component: EditProject },
  { path: '/admin/projects/:id/create-task', component: CreateTask },
  { path: '/admin/projects/:id/tasks/:taskId', component: TaskDetails },
  { path: '/admin/projects/:id/tasks/:taskId/edit', component: EditTask },
  {path: '/my-tasks', component: MyTasks },
  {path: '/my-tasks/:id/logs', component: TaskLogs },
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
    return next('/login');
  }

  next();
});


export default router
