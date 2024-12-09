

kFreqs = [330, 430, 530, 630, 730, 830, 930];
kAmp =   [0.7, 0.75, 0.85, 0.95, 0.75, 0.7, 0.6];
kNoAmp = [0.52, 0.59, 0.49, 0.41, 0.35, 0.35, 0.30];

figure; hold on; grid on;
plot(kFreqs, kAmp, 'k--', 'DisplayName',  'Amplifier', 'LineWidth', 2);
plot(kFreqs, kNoAmp, 'k-', 'DisplayName',  'No amplifier', 'LineWidth', 2);
title('KF Radio Correction');
xlabel('Frequency [Hz]');
ylabel('KF Correction Factor');
ylim([0, 1]);
grid on; legend('location', 'southeast');
saveas(gcf, 'KF.png');

kFreqs = [330, 430, 530, 630, 730, 830, 930];
kAmp =   [0.95, 0.75, 0.55, 0.35, 0.25, 0.55, 0.75];
kNoAmp = [0.05, 0.12, 0.24, 0.39, 0.62, 0.94, 0.82];

figure; hold on; grid on;
plot(kFreqs, kAmp, 'k--', 'DisplayName',  'Amplifier', 'LineWidth', 2);
plot(kFreqs, kNoAmp, 'k-', 'DisplayName',  'No amplifier', 'LineWidth', 2);
title('RF Radio Correction');
xlabel('Frequency [Hz]');
ylabel('RF Correction Factor');
ylim([0, 1]);
grid on; legend('location', 'southeast');
saveas(gcf, 'RF.png');