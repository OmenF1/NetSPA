import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { AddSeries} from "./components/AddSeries";
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/add-series',
    requireAuth: true,
    element: <AddSeries />
  },
  ...ApiAuthorzationRoutes
];

export default AppRoutes;
