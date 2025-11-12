import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';

// https://astro.build/config
export default defineConfig({
  site: 'https://simyunsup.github.io',
  base: '/asp-net-for-frontend/',
  integrations: [
    starlight({
      title: 'ASP.NET Core for Frontend Developers',
      description: 'Frontend Developer\'s Complete Guide to ASP.NET Core',
      defaultLocale: 'root',
      locales: {
        root: {
          label: '한국어',
          lang: 'ko-KR',
        },
      },
      social: [
        {
          icon: 'github',
          label: 'GitHub',
          href: 'https://github.com/SimYunSup/asp-net-for-frontend',
        },
      ],
      editLink: {
        baseUrl: 'https://github.com/SimYunSup/asp-net-for-frontend/edit/main/',
      },
      lastUpdated: true,
      sidebar: [
        {
          label: 'Home',
          link: '/',
        },
        {
          label: 'Part 1: C# Basics',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/part1-csharp-basics/' },
            { label: 'Chapter 1: C# Fundamentals', link: '/part1-csharp-basics/chapter1/' },
            { label: 'Chapter 2: OOP & Advanced Features', link: '/part1-csharp-basics/chapter2/' },
          ],
        },
        {
          label: 'Part 2: ASP.NET Core Basics',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/part2-aspnetcore-basics/' },
            { label: 'Chapter 3: Introduction', link: '/part2-aspnetcore-basics/chapter3/' },
            { label: 'Chapter 4: Core Architecture', link: '/part2-aspnetcore-basics/chapter4/' },
            { label: 'Chapter 5: Minimal APIs', link: '/part2-aspnetcore-basics/chapter2/' },
          ],
        },
        {
          label: 'Part 3: Server-Side Rendering',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/part3-server-side-rendering/' },
            { label: 'Chapter 6: Razor Syntax', link: '/part3-server-side-rendering/chapter6/' },
            { label: 'Chapter 7: Razor Pages', link: '/part3-server-side-rendering/chapter7/' },
            { label: 'Chapter 8: MVC Pattern', link: '/part3-server-side-rendering/chapter8/' },
          ],
        },
        {
          label: 'Part 4: Blazor',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/part4-blazor/' },
            { label: 'Chapter 9: Blazor Introduction', link: '/part4-blazor/chapter9/' },
            { label: 'Chapter 10: Component Development', link: '/part4-blazor/chapter10/' },
            { label: 'Chapter 11: Advanced Patterns', link: '/part4-blazor/chapter11/' },
          ],
        },
        {
          label: 'Part 5: Data Access',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/part5-data-access/' },
            { label: 'Chapter 12: EF Core Basics', link: '/part5-data-access/chapter12/' },
            { label: 'Chapter 13: EF Core Advanced', link: '/part5-data-access/chapter13/' },
          ],
        },
        {
          label: 'Part 6: API Development',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/part6/' },
            { label: 'Chapter 14: RESTful API', link: '/part6/chapter14/' },
            { label: 'Chapter 15: API Security & Auth', link: '/part6/chapter15/' },
            { label: 'Chapter 16: GraphQL & SignalR', link: '/part6/chapter16/' },
          ],
        },
        {
          label: 'Part 7: Real-time & Integration',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/part7/' },
            { label: 'Chapter 17: Real-time Communication', link: '/part7/chapter17/' },
            { label: 'Chapter 18: API Client Patterns', link: '/part7/chapter18/' },
          ],
        },
        {
          label: 'Part 8: State Management & Patterns',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/part8/' },
            { label: 'Chapter 19: Server-Side State', link: '/part8/chapter19/' },
            { label: 'Chapter 20: Architecture Patterns', link: '/part8/chapter20/' },
          ],
        },
        {
          label: 'Part 9: Testing',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/part9/' },
            { label: 'Chapter 21: Unit & Integration Tests', link: '/part9/chapter21/' },
          ],
        },
        {
          label: 'Part 10: Performance & Monitoring',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/part10/' },
            { label: 'Chapter 22: Performance Optimization', link: '/part10/chapter22/' },
            { label: 'Chapter 23: Monitoring & Logging', link: '/part10/chapter23/' },
          ],
        },
        {
          label: 'Part 11: Deployment & DevOps',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/part11/' },
            { label: 'Chapter 24: Docker & Containers', link: '/part11/chapter24/' },
            { label: 'Chapter 25: Cloud Deployment', link: '/part11/chapter25/' },
            { label: 'Chapter 26: Production Considerations', link: '/part11/chapter26/' },
          ],
        },
        {
          label: 'Part 12: Real Project',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/part12/' },
            { label: 'Chapter 27: E-commerce Platform', link: '/part12/chapter27/' },
            { label: 'Chapter 28: Best Practices', link: '/part12/chapter28/' },
          ],
        },
        {
          label: 'Appendices',
          collapsed: true,
          items: [
            { label: 'Overview', link: '/appendices/' },
            { label: 'Appendix A: C# Cheat Sheet', link: '/appendices/appendix-a/' },
            { label: 'Appendix B: Project Templates', link: '/appendices/appendix-b/' },
            { label: 'Appendix C: NuGet Packages', link: '/appendices/appendix-c/' },
            { label: 'Appendix D: Tools & Extensions', link: '/appendices/appendix-d/' },
            { label: 'Appendix E: Learning Resources', link: '/appendices/appendix-e/' },
            { label: 'Appendix F: Migration Guide', link: '/appendices/appendix-f/' },
            { label: 'Appendix G: Troubleshooting', link: '/appendices/appendix-g/' },
          ],
        },
        {
          label: 'Conclusion',
          link: '/conclusion/',
        },
      ],
      customCss: [
        './src/styles/custom.css',
      ],
    }),
  ],
});
