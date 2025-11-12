import { defineConfig } from 'vitepress'

export default defineConfig({
  title: 'ASP.NET Core for Frontend Developers',
  description: 'Frontend Developer\'s Complete Guide to ASP.NET Core',
  // For GitHub Pages: use '/asp-net-for-frontend/' if deploying to username.github.io/asp-net-for-frontend
  // For custom domain or root deployment: use '/'
  base: '/asp-net-for-frontend/',
  lang: 'ko-KR',

  themeConfig: {
    logo: '/logo.svg',

    nav: [
      { text: 'Home', link: '/' },
      { text: 'Part 1: C# Basics', link: '/part1-csharp-basics/README' },
      { text: 'Part 2: ASP.NET Core', link: '/part2-aspnetcore-basics/README' },
      {
        text: 'More Parts',
        items: [
          { text: 'Part 3: Server-Side Rendering', link: '/part3-server-side-rendering/README' },
          { text: 'Part 4: Blazor', link: '/part4-blazor/README' },
          { text: 'Part 5: Data Access', link: '/part5-data-access/README' },
          { text: 'Part 6: API Development', link: '/part6/README' },
          { text: 'Part 7: Real-time & Integration', link: '/part7/README' },
          { text: 'Part 8: State Management', link: '/part8/README' },
          { text: 'Part 9: Testing', link: '/part9/README' },
          { text: 'Part 10: Performance', link: '/part10/README' },
          { text: 'Part 11: Deployment', link: '/part11/README' },
          { text: 'Part 12: Real Project', link: '/part12/README' }
        ]
      },
      { text: 'Appendices', link: '/appendices/README' },
      { text: 'Conclusion', link: '/conclusion' }
    ],

    sidebar: {
      '/part1-csharp-basics/': [
        {
          text: 'Part 1: C# Basics',
          items: [
            { text: 'Overview', link: '/part1-csharp-basics/README' },
            {
              text: 'Chapter 1: C# Fundamentals',
              link: '/part1-csharp-basics/chapter1/README',
              collapsed: true,
              items: [
                { text: 'Async Patterns', link: '/part1-csharp-basics/chapter1/examples/01-async-patterns/README' },
                { text: 'LINQ Basics', link: '/part1-csharp-basics/chapter1/examples/02-linq-basics/README' }
              ]
            },
            {
              text: 'Chapter 2: OOP & Advanced Features',
              link: '/part1-csharp-basics/chapter2/README',
              collapsed: true,
              items: [
                { text: 'Component to Class', link: '/part1-csharp-basics/chapter2/examples/01-oop-patterns/01-component-to-class/README' },
                { text: 'Interfaces', link: '/part1-csharp-basics/chapter2/examples/01-oop-patterns/interfaces/README' },
                { text: 'LINQ Advanced', link: '/part1-csharp-basics/chapter2/examples/02-linq-advanced/README' },
                { text: 'Events & Delegates', link: '/part1-csharp-basics/chapter2/examples/03-events-delegates/README' }
              ]
            }
          ]
        }
      ],
      '/part2-aspnetcore-basics/': [
        {
          text: 'Part 2: ASP.NET Core Basics',
          items: [
            { text: 'Overview', link: '/part2-aspnetcore-basics/README' },
            { text: 'Chapter 3: Introduction', link: '/part2-aspnetcore-basics/chapter3/README' },
            { text: 'Chapter 4: Core Architecture', link: '/part2-aspnetcore-basics/chapter4/README' },
            { text: 'Chapter 5: Minimal APIs', link: '/part2-aspnetcore-basics/chapter2/README' }
          ]
        }
      ],
      '/part3-server-side-rendering/': [
        {
          text: 'Part 3: Server-Side Rendering',
          items: [
            { text: 'Overview', link: '/part3-server-side-rendering/README' },
            { text: 'Chapter 6: Razor Syntax', link: '/part3-server-side-rendering/chapter6/README' },
            { text: 'Chapter 7: Razor Pages', link: '/part3-server-side-rendering/chapter7/README' },
            { text: 'Chapter 8: MVC Pattern', link: '/part3-server-side-rendering/chapter8/README' }
          ]
        }
      ],
      '/part4-blazor/': [
        {
          text: 'Part 4: Blazor',
          items: [
            { text: 'Overview', link: '/part4-blazor/README' },
            { text: 'Chapter 9: Blazor Introduction', link: '/part4-blazor/chapter9/README' },
            { text: 'Chapter 10: Component Development', link: '/part4-blazor/chapter10/README' },
            { text: 'Chapter 11: Advanced Patterns', link: '/part4-blazor/chapter11/README' }
          ]
        }
      ],
      '/part5-data-access/': [
        {
          text: 'Part 5: Data Access',
          items: [
            { text: 'Overview', link: '/part5-data-access/README' },
            { text: 'Chapter 12: EF Core Basics', link: '/part5-data-access/chapter12/README' },
            { text: 'Chapter 13: EF Core Advanced', link: '/part5-data-access/chapter13/README' }
          ]
        }
      ],
      '/part6/': [
        {
          text: 'Part 6: API Development',
          items: [
            { text: 'Overview', link: '/part6/README' },
            { text: 'Chapter 14: RESTful API', link: '/part6/chapter14/README' },
            { text: 'Chapter 15: API Security & Auth', link: '/part6/chapter15/README' },
            { text: 'Chapter 16: GraphQL & SignalR', link: '/part6/chapter16/README' }
          ]
        }
      ],
      '/part7/': [
        {
          text: 'Part 7: Real-time & Integration',
          items: [
            { text: 'Overview', link: '/part7/README' },
            { text: 'Chapter 17: Real-time Communication', link: '/part7/chapter17/README' },
            { text: 'Chapter 18: API Client Patterns', link: '/part7/chapter18/README' }
          ]
        }
      ],
      '/part8/': [
        {
          text: 'Part 8: State Management & Patterns',
          items: [
            { text: 'Overview', link: '/part8/README' },
            { text: 'Chapter 19: Server-Side State', link: '/part8/chapter19/README' },
            { text: 'Chapter 20: Architecture Patterns', link: '/part8/chapter20/README' }
          ]
        }
      ],
      '/part9/': [
        {
          text: 'Part 9: Testing',
          items: [
            { text: 'Overview', link: '/part9/README' },
            { text: 'Chapter 21: Unit & Integration Tests', link: '/part9/chapter21/README' }
          ]
        }
      ],
      '/part10/': [
        {
          text: 'Part 10: Performance & Monitoring',
          items: [
            { text: 'Overview', link: '/part10/README' },
            { text: 'Chapter 22: Performance Optimization', link: '/part10/chapter22/README' },
            { text: 'Chapter 23: Monitoring & Logging', link: '/part10/chapter23/README' }
          ]
        }
      ],
      '/part11/': [
        {
          text: 'Part 11: Deployment & DevOps',
          items: [
            { text: 'Overview', link: '/part11/README' },
            { text: 'Chapter 24: Docker & Containers', link: '/part11/chapter24/README' },
            { text: 'Chapter 25: Cloud Deployment', link: '/part11/chapter25/README' },
            { text: 'Chapter 26: Production Considerations', link: '/part11/chapter26/README' }
          ]
        }
      ],
      '/part12/': [
        {
          text: 'Part 12: Real Project',
          items: [
            { text: 'Overview', link: '/part12/README' },
            { text: 'Chapter 27: E-commerce Platform', link: '/part12/chapter27/README' },
            { text: 'Chapter 28: Best Practices', link: '/part12/chapter28/README' }
          ]
        }
      ],
      '/appendices/': [
        {
          text: 'Appendices',
          items: [
            { text: 'Overview', link: '/appendices/README' },
            { text: 'Appendix A: C# Cheat Sheet', link: '/appendices/appendix-a' },
            { text: 'Appendix B: Project Templates', link: '/appendices/appendix-b' },
            { text: 'Appendix C: NuGet Packages', link: '/appendices/appendix-c' },
            { text: 'Appendix D: Tools & Extensions', link: '/appendices/appendix-d' },
            { text: 'Appendix E: Learning Resources', link: '/appendices/appendix-e' },
            { text: 'Appendix F: Migration Guide', link: '/appendices/appendix-f' },
            { text: 'Appendix G: Troubleshooting', link: '/appendices/appendix-g' }
          ]
        }
      ]
    },

    socialLinks: [
      { icon: 'github', link: 'https://github.com/SimYunSup/asp-net-for-frontend' }
    ],

    footer: {
      message: 'Released under the MIT License.',
      copyright: 'Made with ❤️ for Frontend Developers learning ASP.NET Core'
    },

    search: {
      provider: 'local'
    },

    editLink: {
      pattern: 'https://github.com/SimYunSup/asp-net-for-frontend/edit/main/:path',
      text: 'Edit this page on GitHub'
    },

    lastUpdated: {
      text: 'Last updated',
      formatOptions: {
        dateStyle: 'short',
        timeStyle: 'short'
      }
    }
  }
})
