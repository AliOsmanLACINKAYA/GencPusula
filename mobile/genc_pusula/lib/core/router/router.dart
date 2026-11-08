import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../features/auth/login_screen.dart';
import '../../features/auth/register_screen.dart';
import '../../features/auth/splash_screen.dart';
import '../../features/detail/firsat_detay_screen.dart';
import '../../features/home/home_screen.dart';
import '../../features/profile/profile_screen.dart';
import '../../features/saved/saved_screen.dart';
import '../app_state.dart';
import '../widgets/navigation_shell.dart';

final appRouter = GoRouter(
  initialLocation: '/splash',
  refreshListenable: appState,
  redirect: (context, state) {
    final yol = state.matchedLocation;

    if (!appState.hazir) return yol == '/splash' ? null : '/splash';
    if (!appState.girisYapildi) {
      return (yol == '/login' || yol == '/register') ? null : '/login';
    }
    if (yol == '/splash' || yol == '/login' || yol == '/register') {
      return '/home';
    }
    return null;
  },
  routes: [
    GoRoute(path: '/splash', builder: (context, state) => const SplashScreen()),
    GoRoute(path: '/login', builder: (context, state) => const LoginScreen()),
    GoRoute(
      path: '/register',
      builder: (context, state) => const RegisterScreen(),
    ),
    GoRoute(
      path: '/firsat/:id',
      builder: (context, state) {
        final id = int.tryParse(state.pathParameters['id'] ?? '');
        return FirsatDetayScreen(id: id ?? 0);
      },
    ),
    StatefulShellRoute.indexedStack(
      builder: (context, state, shell) =>
          NavigationShell(navigationShell: shell),
      branches: [
        StatefulShellBranch(
          routes: [
            GoRoute(
              path: '/home',
              builder: (context, state) => const HomeScreen(),
            ),
          ],
        ),
        StatefulShellBranch(
          routes: [
            GoRoute(
              path: '/saved',
              builder: (context, state) => const SavedScreen(),
            ),
          ],
        ),
        StatefulShellBranch(
          routes: [
            GoRoute(
              path: '/profile',
              builder: (context, state) => const ProfileScreen(),
            ),
          ],
        ),
        StatefulShellBranch(
          routes: [
            GoRoute(
              path: '/admin',
              redirect: (context, state) =>
                  appState.yoneticiMi ? null : '/home',
              builder: (context, state) => const _YonetimScreen(),
            ),
          ],
        ),
      ],
    ),
  ],
);

class _YonetimScreen extends StatelessWidget {
  const _YonetimScreen();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Yönetim')),
      body: const Center(
        child: Padding(
          padding: EdgeInsets.all(24),
          child: Text(
            'Fırsat yönetimi Hafta 5’te eklenecek.',
            textAlign: TextAlign.center,
          ),
        ),
      ),
    );
  }
}
