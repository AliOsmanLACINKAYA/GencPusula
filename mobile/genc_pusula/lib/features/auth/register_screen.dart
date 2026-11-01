import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../core/app_state.dart';
import '../../core/utils/validators.dart';
import '../../core/widgets/button.dart';
import '../../core/widgets/scaffold.dart';
import '../../core/widgets/text_field.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final _formKey = GlobalKey<FormState>();
  final _email = TextEditingController();
  final _parola = TextEditingController();
  final _parolaTekrar = TextEditingController();
  bool _gizliParola = true;
  bool _yukleniyor = false;

  @override
  void dispose() {
    _email.dispose();
    _parola.dispose();
    _parolaTekrar.dispose();
    super.dispose();
  }

  Future<void> _kayitOl() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _yukleniyor = true);
    try {
      await appState.kayitOl(_email.text.trim(), _parola.text);
    } on ApiHatasi catch (e) {
      _hata(e.mesaj);
    } catch (_) {
      _hata('Kayıt yapılamadı. Tekrar dene.');
    } finally {
      if (mounted) setState(() => _yukleniyor = false);
    }
  }

  void _hata(String mesaj) {
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(mesaj)));
  }

  @override
  Widget build(BuildContext context) {
    return AuthScaffold(
      showBack: true,
      title: 'Aramıza katıl',
      subtitle: 'Sana uygun fırsatları tek yerden takip et.',
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppTextField(
              controller: _email,
              label: 'E-posta',
              hint: 'ornek@universite.edu.tr',
              prefixIcon: Icons.mail_outline,
              keyboardType: TextInputType.emailAddress,
              textInputAction: TextInputAction.next,
              validator: validateEmail,
            ),
            const SizedBox(height: 14),
            AppTextField(
              controller: _parola,
              label: 'Parola',
              prefixIcon: Icons.lock_outline,
              obscureText: _gizliParola,
              textInputAction: TextInputAction.next,
              validator: validatePassword,
              suffixIcon: IconButton(
                onPressed: () {
                  setState(() => _gizliParola = !_gizliParola);
                },
                icon: Icon(
                  _gizliParola
                      ? Icons.visibility_outlined
                      : Icons.visibility_off_outlined,
                ),
              ),
            ),
            const SizedBox(height: 14),
            AppTextField(
              controller: _parolaTekrar,
              label: 'Parola tekrar',
              prefixIcon: Icons.lock_reset_outlined,
              obscureText: _gizliParola,
              textInputAction: TextInputAction.done,
              onFieldSubmitted: (_) => _kayitOl(),
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Parola tekrarı gerekli';
                }
                if (value != _parola.text) return 'Parolalar eşleşmiyor';
                return null;
              },
              suffixIcon: IconButton(
                onPressed: () {
                  setState(() => _gizliParola = !_gizliParola);
                },
                icon: Icon(
                  _gizliParola
                      ? Icons.visibility_outlined
                      : Icons.visibility_off_outlined,
                ),
              ),
            ),
            const SizedBox(height: 20),
            AppButton(
              label: 'Kayıt ol',
              isLoading: _yukleniyor,
              onPressed: _kayitOl,
            ),
            TextButton(
              onPressed: () {
                if (context.canPop()) {
                  context.pop();
                } else {
                  context.go('/login');
                }
              },
              child: const Text('Hesabın var mı? Giriş yap'),
            ),
          ],
        ),
      ),
    );
  }
}
